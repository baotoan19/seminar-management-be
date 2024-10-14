using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Seminar.APPLICATION.Dtos.ArticleDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Constants;
using Seminar.CORE.ExceptionCustom;
using Seminar.DOMAIN.Entitys;
using Seminar.DOMAIN.Interfaces;

namespace Seminar.APPLICATION.Services;
public class ArticleService : IArticleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ArticleService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ArticleVM> GetArticleByIdAsync(int id)
    {
        Article article = await _unitOfWork.GetRepository<Article>().Entities.Include(a => a.Discipline)
        .FirstOrDefaultAsync(a => a.Id == id)?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Artical not found!");
        ArticleVM articleVM = _mapper.Map<ArticleVM>(article);
        articleVM.DisciplineName = article.Discipline.DisciplineName;
        return articleVM;
    }

    public async Task CreateArticalAsync(CreateArticleDto createArticalsDto)
    {
        Article article = _mapper.Map<Article>(createArticalsDto);
        article.Discipline = await _unitOfWork.GetRepository<Discipline>().GetByIdAsync(createArticalsDto.DisciplineId) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Discipline not found!");
        string keyword = string.Join(",", createArticalsDto.Keywords);
        article.KeyWord = keyword;
        article.IsStatus = false;
        await _unitOfWork.GetRepository<Article>().InsertAsync(article);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateArticalAsync(int id, UpdateArticleDto updateArticalsDto)
    {
        Article article = await _unitOfWork.GetRepository<Article>().GetByIdAsync(id) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Artical not found!");
        article.Discipline = await _unitOfWork.GetRepository<Discipline>().GetByIdAsync(updateArticalsDto.DisciplineId) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Discipline not found!");
        _mapper.Map(updateArticalsDto, article);
        string keyword = string.Join(",", updateArticalsDto.Keywords);
        article.KeyWord = keyword;
        await _unitOfWork.GetRepository<Article>().UpdateAsync(article);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteArticalAsync(int id)
    {
        Article article = await _unitOfWork.GetRepository<Article>().GetByIdAsync(id) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Artical not found!");
        article.DeletedAt = DateTime.Now;
        await _unitOfWork.GetRepository<Article>().UpdateAsync(article);
        await _unitOfWork.SaveChangesAsync();
    }

}