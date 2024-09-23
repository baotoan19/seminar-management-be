using Seminar.APPLICATION.Dtos.OrganizersDtos;
using Seminar.APPLICATION.Models;
using Seminar.DOMAIN.Entitys;

namespace Seminar.APPLICATION.Interfaces.IOrganizerService{
    public interface IOrganizerService{
        Task<Organizer> CreateOrganizerAsync(CreateOrganizerDto createOrganizerDto);
        Task<OrganizerVM> GetOrganizerInforAsync(int id);
        Task<Organizer> UpdateOrganizerAsync(int id, UpdateOrganizerDto updateOrganizerDto);
    }
}