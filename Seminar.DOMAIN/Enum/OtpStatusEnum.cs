namespace Seminar.DOMAIN.Enum;

public enum OtpStatusEnum
{
    Pending = 1, // Chờ xử lý
    Verified = 2, // Đã xác thực
    Expired = 3, // Hết hạn
    Invalid = 4, // Không hợp lệ
    Cancelled = 5 // Đã hủy
}