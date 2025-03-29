using FPTURoomBooking.Repositories.Models;
using FPTURoomBooking.Repositories.Repositories;
using FPTURoomBooking.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FPTURoomBooking.Services.Services
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Booking> GetBookingByIdAsync(int id)
        {
            return await _unitOfWork.Repository<Booking>()
                .GetAll()
                .Include(b => b.Room)
                    .ThenInclude(r => r.Building)
                .Include(b => b.User)
                .SingleOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Booking>> GetAllBookingsAsync()
        {
            return await _unitOfWork.Repository<Booking>()
                .GetAll()
                .Include(b => b.Room)
                    .ThenInclude(r => r.Building)
                .Include(b => b.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByUserAsync(int userId)
        {
            return await _unitOfWork.Repository<Booking>()
                .Find(b => b.UserId == userId)
                .Include(b => b.Room)
                    .ThenInclude(r => r.Building)
                .Include(b => b.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByRoomAsync(int roomId)
        {
            return await _unitOfWork.Repository<Booking>()
                .Find(b => b.RoomId == roomId)
                .Include(b => b.Room)
                    .ThenInclude(r => r.Building)
                .Include(b => b.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _unitOfWork.Repository<Booking>()
                .Find(b => (b.StartTime >= startDate && b.StartTime <= endDate) ||
                           (b.EndTime >= startDate && b.EndTime <= endDate) ||
                           (b.StartTime <= startDate && b.EndTime >= endDate))
                .Include(b => b.Room)
                    .ThenInclude(r => r.Building)
                .Include(b => b.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetPendingBookingsAsync()
        {
            return await _unitOfWork.Repository<Booking>()
                .Find(b => b.Status == "Chờ duyệt")
                .Include(b => b.Room)
                    .ThenInclude(r => r.Building)
                .Include(b => b.User)
                .ToListAsync();
        }

        public async Task<bool> CreateBookingAsync(Booking booking)
        {
            // Kiểm tra xem phòng có sẵn không
            bool isAvailable = await IsRoomAvailableAsync(booking.RoomId, booking.StartTime, booking.EndTime);
            if (!isAvailable)
                return false;

            booking.Status = "Chờ duyệt";
            booking.CreatedAt = DateTime.Now;
            booking.UpdatedAt = DateTime.Now;

            _unitOfWork.Repository<Booking>().Add(booking);
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> UpdateBookingAsync(Booking booking)
        {
            // Kiểm tra xem phòng có sẵn không (loại trừ booking hiện tại)
            bool isAvailable = await IsRoomAvailableAsync(booking.RoomId, booking.StartTime, booking.EndTime, booking.Id);
            if (!isAvailable)
                return false;

            booking.UpdatedAt = DateTime.Now;
            _unitOfWork.Repository<Booking>().Update(booking);
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteBookingAsync(int id)
        {
            var booking = await GetBookingByIdAsync(id);
            if (booking == null) return false;

            _unitOfWork.Repository<Booking>().Remove(booking);
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> ApproveBookingAsync(int id)
        {
            var booking = await GetBookingByIdAsync(id);
            if (booking == null) return false;

            booking.Status = "Đã duyệt";
            booking.UpdatedAt = DateTime.Now;
            _unitOfWork.Repository<Booking>().Update(booking);
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> RejectBookingAsync(int id, string reason)
        {
            var booking = await GetBookingByIdAsync(id);
            if (booking == null) return false;

            booking.Status = "Từ chối";
            booking.Purpose = reason; // Lưu lý do từ chối
            booking.UpdatedAt = DateTime.Now;
            _unitOfWork.Repository<Booking>().Update(booking);
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> CancelBookingAsync(int id, string reason)
        {
            var booking = await GetBookingByIdAsync(id);
            if (booking == null) return false;

            booking.Status = "Đã hủy";
            booking.Purpose = reason; // Lưu lý do hủy
            booking.UpdatedAt = DateTime.Now;
            _unitOfWork.Repository<Booking>().Update(booking);
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime startTime, DateTime endTime, int? excludeBookingId = null)
        {
            var query = _unitOfWork.Repository<Booking>()
                .Find(b => b.Status != "Đã hủy" && b.Status != "Từ chối" &&
                           b.RoomId == roomId &&
                           ((b.StartTime < endTime && b.EndTime > startTime)));

            // Loại trừ booking hiện tại khi đang cập nhật
            if (excludeBookingId.HasValue)
            {
                query = query.Where(b => b.Id != excludeBookingId.Value);
            }

            // Nếu có bất kỳ booking nào trong khoảng thời gian, phòng không khả dụng
            bool hasConflict = await query.AnyAsync();
            return !hasConflict;
        }
    }
}