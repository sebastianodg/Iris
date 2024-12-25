namespace Iris.Base.Abstractions
{
	public interface IIrisIdsManager
	{
		void RegisterId(UInt32 id);
		void UnregisterId(UInt32 id);
		UInt32 GetNewId();
	}
}
