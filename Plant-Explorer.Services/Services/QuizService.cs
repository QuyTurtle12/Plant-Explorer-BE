using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Services.Interface;
using Plant_Explorer.Contract.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plant_Explorer.Repositories.Repositories;
using Microsoft.EntityFrameworkCore;
using Plant_Explorer.Contract.Repositories.ModelViews.Quiz;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Plant_Explorer.Contract.Repositories.PaggingItems;
using Plant_Explorer.Core.ExceptionCustom;

namespace Plant_Explorer.Services.Services
{
    public class QuizService : IQuizService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public const int INACTIVE = 0;
        public const int ACTIVE = 1;

        public QuizService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task CreateQuizAsync(CreateQuizDto newQuiz)
        {
            ValidateQuiz(newQuiz);

            // Mapping model to entity
            Quiz quiz = _mapper.Map<Quiz>(newQuiz);
            quiz.Id = Guid.NewGuid();
            quiz.CreatedTime = DateTimeOffset.UtcNow;

            // Add new quiz to database and save
            await _unitOfWork.GetRepository<Quiz>().InsertAsync(quiz);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteQuizAsync(string id)
        {
            Quiz? existingQuiz = await _unitOfWork.GetRepository<Quiz>().Entities
                .Where(q => q.Id.Equals(Guid.Parse(id)))
                .FirstOrDefaultAsync()
                ?? throw new ErrorException(StatusCodes.Status400BadRequest, "INVALID_INPUT", "This quiz does not exist!");

            // Soft delete
            existingQuiz.Status = INACTIVE;
            existingQuiz.LastUpdatedTime = DateTimeOffset.UtcNow;
            existingQuiz.DeletedTime = existingQuiz.LastUpdatedTime;

            await _unitOfWork.SaveAsync();
        }

        public async Task<PaginatedList<Quiz>> GetAllQuizzesAsync(int index, int pageSize, string? nameSearch)
        {
            if (index <= 0 || pageSize <= 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, "BAD_REQUEST", "Index and PageSize must be greater than 0");
            }

            IQueryable<Quiz> query = _unitOfWork.GetRepository<Quiz>().Entities;

            if (!string.IsNullOrWhiteSpace(nameSearch))
            {
                query = query.Where(q => q.Name.Contains(nameSearch));
            }

            query = query.Where(q => !q.DeletedTime.HasValue).OrderBy(q => q.Name);

            return await _unitOfWork.GetRepository<Quiz>().GetPagging(query, index, pageSize);
        }

        public async Task<Quiz> GetQuizByIdAsync(string id)
        {
            Quiz? quiz = await _unitOfWork.GetRepository<Quiz>().GetByIdAsync(Guid.Parse(id));
            if (quiz == null || quiz.DeletedTime.HasValue)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, "NOT_FOUND", "Quiz not found!");
            }
            return quiz;
        }

        public async Task UpdateQuizAsync(string id, UpdateQuizDto updatedQuiz)
        {
            Quiz? existingQuiz = await _unitOfWork.GetRepository<Quiz>().Entities
                .Where(q => q.Id.Equals(Guid.Parse(id)))
                .FirstOrDefaultAsync()
                ?? throw new ErrorException(StatusCodes.Status400BadRequest, "INVALID_INPUT", "This quiz does not exist!");

            ValidateQuiz(updatedQuiz);

            _mapper.Map(updatedQuiz, existingQuiz);
            existingQuiz.LastUpdatedTime = DateTimeOffset.UtcNow;

            _unitOfWork.GetRepository<Quiz>().Update(existingQuiz);
            await _unitOfWork.SaveAsync();
        }

        private void ValidateQuiz(dynamic quiz)
        {
            if (string.IsNullOrWhiteSpace(quiz.Name))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, "INVALID_INPUT", "Name must not be empty!");
            }

            if (quiz.Status != ACTIVE && quiz.Status != INACTIVE)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, "INVALID_INPUT", "Invalid status value!");
            }
        }
    }
}
    
