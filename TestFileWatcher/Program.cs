﻿//
// Program.cs
//
// Author:
//       Matt Ward <matt.ward@microsoft.com>
//
// Copyright (c) 2018 Microsoft
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.IO;

namespace TestFileWatcher
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			try {
				Run (args);
				Console.ReadKey ();
			} catch (Exception ex) {
				Console.WriteLine (ex);
			}
		}

		static void Run (string[] args)
		{
			if (args.Length < 1) {
				ShowUsage ();
				return;
			}

			string path = args [0];
			if (!Directory.Exists (path)) {
				Console.WriteLine ("Directory does not exist.");
				return;
			}

			if (args.Length > 1) {
				if (!ConfigureFileWatcherPlatform (args [1])) {
					ShowUsage ();
					return;
				}
			}

			if (MonoDevelop.FSW.FileSystemWatcher.IsOSX) {
				Console.WriteLine ("Using native OSX file watcher.");
			} else {
				Console.WriteLine ("Using .NET file watcher.");
			}

			var watcher = new MonoDevelop.FSW.FileSystemWatcher (path);
			watcher.IncludeSubdirectories = true;
			watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
			watcher.Changed += Watcher_Changed;
			watcher.Created += Watcher_Created;
			watcher.Deleted += Watcher_Deleted;
			watcher.Renamed += Watcher_Renamed;
			watcher.Error += Watcher_Error;

			watcher.EnableRaisingEvents = true;
		}

		static void ShowUsage ()
		{
			Console.WriteLine ("TestFileWatcher directory -type:mono|osx");
		}

		static bool ConfigureFileWatcherPlatform (string arg)
		{
			const string typeArgument = "-type:";
			if (!arg.StartsWith (typeArgument, StringComparison.OrdinalIgnoreCase)) {
				return false;
			}

			arg = arg.Substring (typeArgument.Length);
			if (!Enum.TryParse (arg, true, out MonoDevelop.FSW.FileSystemWatcher.Platform platform)) {
				return false;
			}

			MonoDevelop.FSW.FileSystemWatcher.CurrentPlatform = platform;

			return true;
		}

		static void Watcher_Changed (object sender, FileSystemEventArgs e)
		{
			Console.WriteLine ("Changed: {0}", e.Name);
		}

		static void Watcher_Created (object sender, FileSystemEventArgs e)
		{
			Console.WriteLine ("Created: {0}", e.Name);
		}

		static void Watcher_Deleted (object sender, FileSystemEventArgs e)
		{
			Console.WriteLine ("Deleted: {0}", e.Name);
		}

		static void Watcher_Error (object sender, ErrorEventArgs e)
		{
			Console.WriteLine ("Error: {0}", e.GetException ());
		}

		static void Watcher_Renamed (object sender, RenamedEventArgs e)
		{
			Console.WriteLine ("Renamed: {0} -> {1}", e.OldName, e.Name);
		}
	}
}
