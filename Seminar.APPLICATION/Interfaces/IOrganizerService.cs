using Seminar.APPLICATION.Dtos.OrganizersDtos;
using Seminar.DOMAIN.Entitys;

namespace Seminar.APPLICATION.Interfaces.IOrganizerService{
    public interface IOrganizerService{
        Task<Organizer> CreateOrganizerAsync(CreateOrganizerDto createOrganizerDto);
    }
}