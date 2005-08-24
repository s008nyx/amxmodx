using System;
using System.Diagnostics;
using System.IO;

namespace AMXXRelease
{
	//Build process for Windows (32bit)
	public class Win32Builder : ABuilder
	{
		private string m_AmxxPc;

		public override void OnBuild()
		{
			m_AmxxPc = PropSlashes(m_Cfg.GetSourceTree() + "\\plugins\\amxxpc.exe");
		}

		public override void CompressDir(string target, string dir)
		{
			ProcessStartInfo info = new ProcessStartInfo();

			info.FileName = m_Cfg.CompressPath();
			info.WorkingDirectory = m_Cfg.OutputPath();
			info.Arguments = "-r " + target + " " + dir;
			info.UseShellExecute = false;

			Process p = Process.Start(info);
			p.WaitForExit();
		}

		public override void AmxxPc(string inpath, string args)
		{
			ProcessStartInfo info = new ProcessStartInfo();

			info.WorkingDirectory = PropSlashes(m_Cfg.GetSourceTree() + "\\plugins");
			info.FileName = (string)m_AmxxPc.Clone();
			info.Arguments = inpath + ".sma";
			if (args != null)
				info.Arguments += " " + args;
			info.UseShellExecute = false;

			Process p = Process.Start(info);
			p.WaitForExit();
		}

		public override string GetLibExt()
		{
			return ".dll";
		}

		public override string BuildModule(Module module)
		{
			ProcessStartInfo info = new ProcessStartInfo();

			string dir = m_Cfg.GetSourceTree() + "\\" + module.sourcedir;
			if (module.bindir != null)
				dir += "\\" + module.bindir;
			string file = dir;
			if (module.bindir == null)
				file += "\\" + module.bindir;
			file += "\\" + module.build + "\\" + module.projname + ".dll";
			file = PropSlashes(file);

			if (File.Exists(file))
				File.Delete(file);

			info.WorkingDirectory = PropSlashes(dir);
			info.FileName = m_Cfg.DevenvPath();
			info.Arguments = "/build " + module.build + " " + module.vcproj + ".vcproj";
			info.UseShellExecute = false;

			Process p = Process.Start(info);
			p.WaitForExit();

			if (!File.Exists(file))
				return null;

			return file;
		}
	}
}
