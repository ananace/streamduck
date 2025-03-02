using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using NLog;

namespace Streamduck.Plugins.Loaders;

public static class PluginLoader {
	private static readonly Logger L = LogManager.GetCurrentClassLogger();

	public static PluginAssembly? Load(string path) {
		var curDir = Directory.GetCurrentDirectory();
		var fullPath = Path.Combine(curDir, path);

		var context = new PluginLoadContext(fullPath);

		try {
			var plugins = CreatePlugins(context, LoadPlugin(context, fullPath));
			return new PluginAssembly(context, plugins.ToArray());
		} catch (Exception e) {
			L.Error("Failed to load plugin at {0}\nReason: {1}", path, e);
			return null;
		}
	}

	public static IEnumerable<PluginAssembly> LoadFromFolder(string pathToFolder) {
		L.Info("Loading plugins in {0} folder...", pathToFolder);

		var curDir = Directory.GetCurrentDirectory();
		var fullPath = Path.Combine(curDir, pathToFolder);

		foreach (var filePath in Directory.GetFiles(pathToFolder)) {
			if (!filePath.EndsWith("dll")) continue;

			var assembly = Load(filePath);

			if (assembly == null) continue;

			yield return assembly;
		}

		foreach (var directory in Directory.GetDirectories(pathToFolder)) {
			var directoryName = Path.GetFileName(directory);

			var dllPath = Path.Combine(directory, $"{directoryName}.dll");

			var assembly = Load(dllPath);

			if (assembly == null) continue;

			yield return assembly;
		}
	}

	private static Assembly LoadPlugin(AssemblyLoadContext context, string assemblyPath) =>
		context.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(assemblyPath)));

	private static IEnumerable<WrappedPlugin> CreatePlugins(PluginLoadContext context, Assembly assembly) {
		var loadedPlugins = 0;

		foreach (var type in assembly.GetTypes()) {
			if (!typeof(Plugin).IsAssignableFrom(type)) continue;

			if (Activator.CreateInstance(type) is not Plugin plugin) continue;

			L.Info("Loading plugin \"{0}\"...", plugin.Name);

			loadedPlugins++;
			var wrapped = new WrappedPlugin(plugin, context);

			L.Info("Loaded plugin \"{0}\" ({1})", plugin.Name, assembly.Location);

			yield return wrapped;
		}

		if (loadedPlugins != 0) yield break;

		var types = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
		throw new ApplicationException(
			$"{assembly} ({assembly.Location}) doesn't have any types that implement Plugin class\n" +
			$"Available types: {types}");
	}
}