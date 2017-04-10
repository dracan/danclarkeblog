namespace DanClarkeBlog.Core.Models
{
	/// <summary>
	/// Reference class to contain a cursor. Used so we can pass cursor as parameter and have it updated.
	/// Unfortunately async methods do not support ref parameters.
	/// </summary>
	public class CursorContainer
	{
		public string Cursor { get; set; }
	}
}