using System.Threading.Tasks;

namespace Streamduck.Definitions.Scripting; 

public abstract class ScriptingSystem {
	public abstract Task<Script> New();
	public abstract Task<Script> Deserialize(byte[] data);
}