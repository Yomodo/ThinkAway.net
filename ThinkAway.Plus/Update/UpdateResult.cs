
namespace ThinkAway.Plus.Update
{
	/// <summary>
	/// 更新结果
	/// </summary>
	public class UpdateResult
	{
		/// <summary>
		/// 较新的产品列表
		/// </summary>
		public Products Products { get; set; }
		/// <summary>
		/// 错误信息
		/// </summary>
		public string Error { get; set; }
	}
}
