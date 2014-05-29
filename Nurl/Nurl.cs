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
		public void TestIfCommands(){
			Nurl badUrl  = new Nurl(new string[]{"test", "get", "-save", "-times"});
			Nurl goodUrl = new Nurl(new string[]{"get", "-url","http://toto.com","-times"});
			
			Assert.AreEqual(false, badUrl.parseCommand());
			Assert.AreEqual(true, goodUrl.parseCommand());
		}
		
		[Test]
		public void TestGetHtml(){
			Nurl goodUrl = new Nurl(new string[]{"get", "-url","http://toto.com","-times"});
			Assert.AreEqual(goodUrl.getUrl(),goodUrl.getUrl());
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
		public Nurl( string[] commandIn)
		{
			command = commandIn;
			commandsToExecute = new List<string>();
			if(!parseCommand()) Console.WriteLine("error in parsing");
			
		}
		
		public bool parseCommand(){
			if(-1 != Array.IndexOf(features, command[0]))
			{
				commandsToExecute.Add(command[0]);
			}else return false;
			
			for(int i = 1; i < command.Length; i+=2){
				if(-1 != Array.IndexOf(options, (command[i].ToLower())))
				{
					commandsToExecute.Add(command[i].ToLower());

					continue;
				}
				return false;
			}
			return true;
			
		}
		
		public string getUrl(){
			string feature = commandsToExecute[0];
			StringBuilder sb = new StringBuilder();
			Byte[] buf = new byte[8192];
			Stream resStream;
			int count = 0;
			if( (( feature).Equals("get")) && (commandsToExecute.Contains("-url")))
			{
				int urlIndex = Array.IndexOf(command, "-url");
				myHttpWebRequest=(HttpWebRequest)WebRequest.Create(command[urlIndex+1]);
				myHttpWebResponse=(HttpWebResponse)myHttpWebRequest.GetResponse();
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
		
		
		
		public static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			
		}
	}
}
