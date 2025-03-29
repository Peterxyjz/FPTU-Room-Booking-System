using FPTURoomBooking.Repositories.Models;
using FPTURoomBooking.Repositories.Repositories;
using FPTURoomBooking.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPTURoomBooking.Services.Services
{
    public class RoomService : IRoomService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoomService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Room> GetRoomByIdAsync(int id)
        {
            return await _unitOfWork.Repository<Room>().GetByIdAsync(id);
        }

        public async Task<IEnumerable<Room>> GetAllRoomsAsync()
        {
            return await _unitOfWork.Repository<Room>()
                .GetAll()
                .Include(r => r.Building)
                .Include(r => r.RoomEquipments)
                    .ThenInclude(re => re.Equipment)
                .ToListAsync();
        }

        public async Task<IEnumerable<Room>> GetRoomsByBuildingAsync(int buildingId)
        {
            return await _unitOfWork.Repository<Room>()
                .Find(r => r.BuildingId == buildingId)
                .Include(r => r.Building)
                .Include(r => r.RoomEquipments)
                    .ThenInclude(re => re.Equipment)
                .ToListAsync();
        }

        public async Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime startTime, DateTime endTime, int? capacity = null)
        {
            // Lấy tất cả phòng đang được đặt trong khoảng thời gian
            var bookedRoomIds = await _unitOfWork.Repository<Booking>()
                .Find(b => b.Status != "Đã hủy"
                    && ((b.StartTime <= endTime && b.EndTime >= startTime)))
                .Select(b => b.RoomId)
                .ToListAsync();

            // Lấy các phòng không bị đặt và thỏa mãn điều kiện khác
            var query = _unitOfWork.Repository<Room>()
                .Find(r => !bookedRoomIds.Contains(r.Id) && r.Status == "Sẵn sàng");

            // Thêm điều kiện về sức chứa nếu có
            if (capacity.HasValue)
            {
                query = query.Where(r => r.Capacity >= capacity.Value);
            }

            return await query
                .Include(r => r.Building)
                .Include(r => r.RoomEquipments)
                    .ThenInclude(re => re.Equipment)
                .ToListAsync();
        }

        public async Task<bool> CreateRoomAsync(Room room)
        {
            _unitOfWork.Repository<Room>().Add(room);
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> UpdateRoomAsync(Room room)
        {
            _unitOfWork.Repository<Room>().Update(room);
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteRoomAsync(int id)
        {
            var room = await GetRoomByIdAsync(id);
            if (room == null) return false;

            _unitOfWork.Repository<Room>().Remove(room);
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> UpdateRoomStatusAsync(int id, string status)
        {
            var room = await GetRoomByIdAsync(id);
            if (room == null) return false;

            room.Status = status;
            _unitOfWork.Repository<Room>().Update(room);
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0;
        }
    }
}
