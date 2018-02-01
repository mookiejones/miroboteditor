using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InlineFormParser.Model;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InlineFormParser.Tests
{
	[TestClass]
	public class ReadInlineFormTests
	{
		const string PATH=@"D:\kop\SERVOGUN FC\KOP\ServoGun FC\Kxr";

		[TestMethod]
		public void LoadInlineForm()
		{
			var files = Directory.GetFiles(PATH, "*.kxr");
			var map = new TextEntryMap(new[]{"en-us"
		});
		foreach (var file in files)
			{
				var source = new FileSystemSource(file);
				var reader = new KxrReader(source);
				var name = Path.GetFileNameWithoutExtension(file);
				if (name.Contains("."))
					name = Path.GetFileNameWithoutExtension(name);
				string shortcut = "hello";
			 reader.ReadModule(name,map, out shortcut);
			}

	 
		}

	}
}
