using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Seminar.APPLICATION.Auth;
using Seminar.APPLICATION.Dtos.AuthorDtos;
using Seminar.APPLICATION.Interfaces;
using Seminar.APPLICATION.Models;
using Seminar.CORE.Constants;
using Seminar.CORE.ExceptionCustom;
using Seminar.DOMAIN.Entitys;
using Seminar.DOMAIN.Interfaces;

namespace Seminar.APPLICATION.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthorService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccountService _accountService;

        public AuthorService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AuthorService> logger, IHttpContextAccessor httpContextAccessor, IAccountService accountService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _accountService = accountService;
        }

        public async Task<Author> CreateAuthorAsync(CreateAuthorDto createAuthorDto)
        {
            Author? existsAuthor = await _unitOfWork.GetRepository<Author>().Entities.FirstOrDefaultAsync(a => a.AccountId == createAuthorDto.AccountId);
            if (existsAuthor != null)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.EXISTED, "Author is existed!");
            }

            Author author = _mapper.Map<Author>(createAuthorDto);
            await _unitOfWork.GetRepository<Author>().InsertAsync(author);
            await _unitOfWork.SaveChangesAsync();
            return author;
        }

        public async Task<AuthorVM> GetAuthorInforAsync(int id)
        {
            Author? author = await _unitOfWork.GetRepository<Author>().Entities.Include(a => a.Account).Include(a => a.Faculty).FirstOrDefaultAsync(a => a.AccountId == id) ??
            throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Author not found!");
            AuthorVM authorVM = _mapper.Map<AuthorVM>(author);
            authorVM.FacultyName = author.Faculty?.FacultyName ?? null;
            authorVM.Email = author.Account.Email;
            return authorVM;
        }

        public async Task UpdateAuthorAsync(int accountId, UpdateAuthorDto updateAuthorDto)
        {
            Author? author = await _unitOfWork.GetRepository<Author>().Entities.Include(a => a.Account).Include(a => a.Faculty).FirstOrDefaultAsync(a => a.AccountId == accountId) ??
            throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Author not found!");
            _mapper.Map(updateAuthorDto, author);
            if (await _accountService.IsEmailUniqueAsync(updateAuthorDto.Email, accountId))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.EXISTED, "Email is existed!");
            }
            author.Account.Email = updateAuthorDto.Email;
            author.Account.UpdatedAt = DateTime.Now;
            author.UpdatedAt = DateTime.Now;
            await _unitOfWork.GetRepository<Author>().UpdateAsync(author);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task CreateCoAuthorAsync(CreateAuthorDto createAuthorDto)
        {
            Author author = _mapper.Map<Author>(createAuthorDto);
            author.AccountId = null;
            await _unitOfWork.GetRepository<Author>().InsertAsync(author);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateCoAuthorAsync(int idCoAuthor, UpdateAuthorDto updateAuthorDto)
        {
            Author? author = await _unitOfWork.GetRepository<Author>().Entities.FirstOrDefaultAsync(a => a.Id == idCoAuthor) ??
            throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Co-Author not found!");
            _mapper.Map(updateAuthorDto, author);
            await _unitOfWork.GetRepository<Author>().UpdateAsync(author);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteCoAuthorAsync(int idCoAuthor)
        {
            Author? author = await _unitOfWork.GetRepository<Author>().Entities.FirstOrDefaultAsync(a => a.Id == idCoAuthor) ??
            throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Co-Author not found!");
            await _unitOfWork.GetRepository<Author>().DeleteAsync(author.Id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<AuthorVM> GetCoAuthorByIdAsync(int idCoAuthor)
        {
            Author? author = await _unitOfWork.GetRepository<Author>().Entities.Include(a => a.Faculty).FirstOrDefaultAsync(a => a.Id == idCoAuthor) ??
            throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Co-Author not found!");
            AuthorVM authorVM = _mapper.Map<AuthorVM>(author);
            authorVM.FacultyName = author.Faculty?.FacultyName ?? null;
            return authorVM;
        }

    }
}