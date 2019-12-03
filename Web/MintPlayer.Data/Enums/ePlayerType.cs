using System.Runtime.Serialization;

namespace MintPlayer.Data.Enums
{
	public enum ePlayerType
	{
		[EnumMember(Value = "None")]
		None = 0,
		[EnumMember(Value = "YouTube")]
		YouTube = 1,
		[EnumMember(Value = "DailyMotion")]
		DailyMotion = 2
	}
}
