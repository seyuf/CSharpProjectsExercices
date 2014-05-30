/*
 * Created by SharpDevelop.
 * User: madalien
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Text;
using System.Net;
using System.Diagnostics;
using System.Collections.Generic;
using NUnit.Framework;

namespace Nurl
{
	/// <summary>
	///  tests to validate my emulations of NURL command
	/// </summary>
	
	[TestFixture]
	public class TestNurl
	{
		[Test]
		public void FirstTest(){
			Assert.AreEqual("toto", "toto");
		}
		
		[Test]
		public void CanBePArsed(){
			Nurl badUrl  = new Nurl(new string[]{"test", "get", "-save", "-times"});
			Nurl missingArgForOption = new Nurl(new string[]{"get", "-url","http://toto.com","-times"});
			Nurl goodUrl = new Nurl(new string[]{"get", "-url","http://toto.com","-times","3"});
			Assert.AreEqual(false, badUrl.ParseCommand());
			Assert.AreEqual(true, goodUrl.ParseCommand());
			Assert.AreEqual(false, missingArgForOption.ParseCommand());
		}
		
		[Test]
		public void IsScenario(){
			Nurl badScenario  = new Nurl(new string[]{"test", "get", "-save", "-times"});
			Nurl saveScenario  = new Nurl(new string[]{ "get", "-url", "http://toto.com", "-save", "toto.txt"});
			Nurl timeScenario = new Nurl(new string[]{"test", "-url", "http://toto.com","-times"});
			Nurl timeAvgScenario = new Nurl(new string[]{"test", "-url", "http://toto.com","-times", "5", "-avg"});
			Assert.AreEqual(false, badScenario.CheckScenario());
			Assert.AreEqual(true, saveScenario.CheckScenario());
			Assert.AreEqual(true, timeScenario.CheckScenario());
			Assert.AreEqual(true, timeAvgScenario.CheckScenario());
		}
		
		[Test]
		public void TestGetHtml(){
			Nurl goodUrl = new Nurl(new string[]{"get", "-url","http://toto.com","-times"});
			Assert.AreEqual(goodUrl.GetUrl(),goodUrl.GetUrl());
		}
		
		[Test]
		public void TestTimes(){
			Nurl goodUrl = new Nurl(new string[]{"get", "-url","http://toto.com","-times","1"});
			string[] downloadTime = goodUrl.TestUrlTimes().Split(' ');
			Assert.AreEqual(downloadTime.Length, (goodUrl.TestUrlTimes().Split(' ')).Length, goodUrl.TestUrlTimes());
			Console.WriteLine("timeOne: {0}, timeTwo: {1}, \n times: {2}", downloadTime.Length,
			                  (goodUrl.TestUrlTimes().Split(' ')).Length,
			                  (goodUrl.TestUrlTimes()) );
		}
	}
	public class Nurl
	{
		static string [] features = {"get", "test"};
		static string [] options = {"-save", "-times","-avg", "-url"};
		string[] command;
		HttpWebRequest myHttpWebRequest;
		HttpWebResponse myHttpWebResponse;
		public List<string> commandsToExecute;
		public string testScenario;
		
		public Nurl( string[] commandIn)
		{
			command = commandIn;
			commandsToExecute = new List<string>();
			if((!ParseCommand()) || (!CheckScenario())) Console.WriteLine("error in parsing");
			
		}
		
		public bool ParseCommand(){
			if(-1 != Array.IndexOf(features, command[0]))
			{
				commandsToExecute.Add(command[0]);
			}else return false;
			int i = 0;
			for(i = 1; i < command.Length; i+=2){
				if(-1 != Array.IndexOf(options, (command[i].ToLower())))
				{
					commandsToExecute.Add(command[i].ToLower());
					continue;
				}
				return false;
			}
			try{
				string last = command[i - 1];
				return true;
			}catch(Exception ex){
				Console.WriteLine("error in parsing"+ex.Message);
				return false;
			}
			
		}
		
		public bool CheckScenario(){
			string feature = commandsToExecute[0];
			var capitalizer = System.Globalization.CultureInfo.CurrentCulture.TextInfo;
			testScenario = (capitalizer.ToTitleCase(commandsToExecute[0]))+OptionsToString();
			if( null != this.GetType().GetMethod((capitalizer.ToTitleCase(commandsToExecute[0]))+OptionsToString() )){
				return true;
			}else return false;
		}
		
		public string OptionsToString(){
			StringBuilder sb = new StringBuilder();
			var capitalizer = System.Globalization.CultureInfo.CurrentCulture.TextInfo;
			for(int i = 1; i < commandsToExecute.Count; i++){
				sb.Append(capitalizer.ToTitleCase(commandsToExecute[i].Trim( new Char[] { ' ', '-' } )));
			}
			return sb.ToString();
		}
		
		public string GetUrl(){
			string feature = commandsToExecute[0];
			StringBuilder sb = new StringBuilder();
			Byte[] buf = new byte[8192];
			Stream resStream;
			int count = 0;
			if( (( feature).Equals("get")) && (commandsToExecute.Contains("-url")))
			{
				try{
					int urlIndex = Array.IndexOf(command, "-url");
					myHttpWebRequest=(HttpWebRequest)WebRequest.Create(command[urlIndex+1]);
					myHttpWebResponse=(HttpWebResponse)myHttpWebRequest.GetResponse();
				}catch(Exception ex){
					return "issue with the requested url"+ex.Message;
				}
			}
			
			resStream = myHttpWebResponse.GetResponseStream();

			do
			{
				count = resStream.Read(buf, 0, buf.Length);
				if(count != 0)
				{
					sb.Append(Encoding.UTF8.GetString(buf,0,count)); // just hardcoding UTF8 here
				}
			}while (count > 0);
			String html = sb.ToString();
			return html;
		}
		
		public void GetUrlSave(){
			Console.WriteLine("in save");
		}
		public string TestUrlTimes(){
			int urlIndex = Array.IndexOf(command, "-times");
			int times = Convert.ToInt32( command[urlIndex+1] );
			StringBuilder sb = new StringBuilder();
			
			for(int i = 0; i < times; i++){
				Stopwatch downloadTime = new Stopwatch();
				downloadTime.Start();
				StringBuilder sbHtml = new StringBuilder(GetUrl());
				downloadTime.Stop();
				sb.Append(" "+downloadTime.ElapsedMilliseconds+"ms");
			}
			return sb.ToString();
		}
		
		public void TestUrlTimesAvg(){ Console.WriteLine("in save"); }
		
		public static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			
		}
	}
}

