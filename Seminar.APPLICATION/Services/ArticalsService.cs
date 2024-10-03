using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Seminar.APPLICATION.Dtos.ArticalsDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Constants;
using Seminar.CORE.ExceptionCustom;
using Seminar.DOMAIN.Entitys;
using Seminar.DOMAIN.Interfaces;

namespace Seminar.APPLICATION.Services;
public class ArticalService : IArticalsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ArticalService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ArticalVM> GetArticalByIdAsync(int id)
    {
        Artical artical = await _unitOfWork.GetRepository<Artical>().Entities.Include(a => a.Discipline).Include(a => a.Proceeding).Include(a => a.Conference).FirstOrDefaultAsync(a => a.Id == id)
        ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Artical not found!");
        ArticalVM articalVM = _mapper.Map<ArticalVM>(artical);
        articalVM.DisciplineName = artical.Discipline.DisciplineName;
        articalVM.ProceedingTitle = artical.Proceeding.Title;
        articalVM.ConferenceName = artical.Conference.ConferenceName;
        return articalVM;
    }

    public async Task CreateArticalAsync(CUArticalsDto createArticalsDto)
    {
        Artical artical = _mapper.Map<Artical>(createArticalsDto);
        artical.Discipline = await _unitOfWork.GetRepository<Discipline>().GetByIdAsync(createArticalsDto.DisciplineId) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Discipline not found!");
        artical.Proceeding = await _unitOfWork.GetRepository<Proceeding>().GetByIdAsync(createArticalsDto.ProceedingId) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Proceeding not found!");
        artical.Conference = await _unitOfWork.GetRepository<Conference>().GetByIdAsync(createArticalsDto.ConferenceId) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Conference not found!");
        string keyword = string.Join(",", createArticalsDto.Keywords);
        artical.KeyWord = keyword;
        await _unitOfWork.GetRepository<Artical>().InsertAsync(artical);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateArticalAsync(int id, CUArticalsDto updateArticalsDto)
    {
        Artical artical = await _unitOfWork.GetRepository<Artical>().GetByIdAsync(id) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Artical not found!");
        artical.Discipline = await _unitOfWork.GetRepository<Discipline>().GetByIdAsync(updateArticalsDto.DisciplineId) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Discipline not found!");
        artical.Proceeding = await _unitOfWork.GetRepository<Proceeding>().GetByIdAsync(updateArticalsDto.ProceedingId) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Proceeding not found!");
        artical.Conference = await _unitOfWork.GetRepository<Conference>().GetByIdAsync(updateArticalsDto.ConferenceId) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Conference not found!");
        _mapper.Map(updateArticalsDto, artical);
        string keyword = string.Join(",", updateArticalsDto.Keywords);
        artical.KeyWord = keyword;
        await _unitOfWork.GetRepository<Artical>().UpdateAsync(artical);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteArticalAsync(int id)
    {
        Artical artical = await _unitOfWork.GetRepository<Artical>().GetByIdAsync(id) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Artical not found!");
        artical.DeletedAt = DateTime.Now;
        await _unitOfWork.GetRepository<Artical>().UpdateAsync(artical);
        await _unitOfWork.SaveChangesAsync();
    }

}