using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLearningApp.Controllers;
using ServiceLearningApp.Data;
using ServiceLearningApp.Interfaces;
using ServiceLearningApp.Model;
using ServiceLearningApp.Model.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ServiceLearningApp.Tests.Controllers
{
    public class ExerciseTransactionControllerTests
    {
        [Fact]
        public async Task PostAsync_ShouldAddExerciseTransaction()
        {
            // Arrange
            var fakeExerciseTransactionRepository = A.Fake<IExerciseTransactionRepository>();
            var fakeMapper = A.Fake<IMapper>();

            var controller = new ExerciseTransactionController(fakeExerciseTransactionRepository, fakeMapper);

            var questions = new List<Question>
            {
                new Question { Id = 1, QuestionText = "Question 1", FkSubChapterId = 1, Options = new List<Option>() },
                new Question { Id = 2, QuestionText = "Question 2", FkSubChapterId = 1, Options = new List<Option>() },
                new Question { Id = 3, QuestionText = "Question 3", FkSubChapterId = 1, Options = new List<Option>() },
                new Question { Id = 4, QuestionText = "Question 4", FkSubChapterId = 1, Options = new List<Option>() },
                new Question { Id = 5, QuestionText = "Question 5", FkSubChapterId = 1, Options = new List<Option>() },
                new Question { Id = 6, QuestionText = "Question 6", FkSubChapterId = 1, Options = new List<Option>() },
                new Question { Id = 7, QuestionText = "Question 7", FkSubChapterId = 1, Options = new List<Option>() },
                new Question { Id = 8, QuestionText = "Question 8", FkSubChapterId = 1, Options = new List<Option>() },
                new Question { Id = 9, QuestionText = "Question 9", FkSubChapterId = 1, Options = new List<Option>() },
                new Question { Id = 10, QuestionText = "Question 10", FkSubChapterId = 1, Options = new List<Option>() },
            };

            // Inisialisasi data opsi (options)
            var options = new List<Option>
            {
                new Option { Id = 1, OptionText = "Option 1", IsAnswer = true, FkQuestionId = 1 },
                new Option { Id = 2, OptionText = "Option 2", IsAnswer = true, FkQuestionId = 2 },
                new Option { Id = 3, OptionText = "Option 3", IsAnswer = true, FkQuestionId = 3 },
                new Option { Id = 4, OptionText = "Option 4", IsAnswer = true, FkQuestionId = 4 },
                new Option { Id = 5, OptionText = "Option 5", IsAnswer = true, FkQuestionId = 5 },
                new Option { Id = 6, OptionText = "Option 6", IsAnswer = true, FkQuestionId = 6 },
                new Option { Id = 7, OptionText = "Option 7", IsAnswer = true, FkQuestionId = 7 },
                new Option { Id = 8, OptionText = "Option 8", IsAnswer = true, FkQuestionId = 8 },
                new Option { Id = 9, OptionText = "Option 9", IsAnswer = true, FkQuestionId = 9 },
                new Option { Id = 10, OptionText = "Option 10", IsAnswer = true, FkQuestionId = 10 },
            };

            foreach (var question in questions)
            {
                question.Options = options.Where(o => o.FkQuestionId == question.Id).ToList();
            }

            // Inisialisasi data ExerciseTransaction
            var exerciseTransaction = new ExerciseTransaction
            {
                Id = 1,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMinutes(10),
                CorrectAnswer = 8,
                IncorrectAnswer = 2,
                FkSubChapterId = 1,
                FkUserId = "276b2f75-655a-4ac9-802d-ed06323d5cc6",
                HistoryAnswer = new List<HistoryAnswer>
                {
                    new HistoryAnswer { Id = 1, FkQuestionId = 1, Question = questions[0], FkOptionId = 1, Option = options[0], FkExerciseTransactionId = 1 },
                    new HistoryAnswer { Id = 2, FkQuestionId = 2, Question = questions[1], FkOptionId = 2, Option = options[1], FkExerciseTransactionId = 1 },
                    new HistoryAnswer { Id = 3, FkQuestionId = 3, Question = questions[2], FkOptionId = 3, Option = options[2], FkExerciseTransactionId = 1 },
                    new HistoryAnswer { Id = 4, FkQuestionId = 4, Question = questions[3], FkOptionId = 4, Option = options[3], FkExerciseTransactionId = 1 },
                    new HistoryAnswer { Id = 5, FkQuestionId = 5, Question = questions[4], FkOptionId = 5, Option = options[4], FkExerciseTransactionId = 1 },
                    new HistoryAnswer { Id = 6, FkQuestionId = 6, Question = questions[5], FkOptionId = 6, Option = options[5], FkExerciseTransactionId = 1 },
                    new HistoryAnswer { Id = 7, FkQuestionId = 7, Question = questions[6], FkOptionId = 7, Option = options[6], FkExerciseTransactionId = 1 },
                    new HistoryAnswer { Id = 8, FkQuestionId = 8, Question = questions[7], FkOptionId = 8, Option = options[7], FkExerciseTransactionId = 1 },
                    new HistoryAnswer { Id = 9, FkQuestionId = 9, Question = questions[8], FkOptionId = 9, Option = options[8], FkExerciseTransactionId = 1 },
                    new HistoryAnswer { Id = 10, FkQuestionId = 10, Question = questions[9], FkOptionId = 10, Option = options[9], FkExerciseTransactionId = 1 },
                },
                SubChapter = new SubChapter
                {
                    Id = 1,
                    Title = "Image Editing"
                },
                User = new ApplicationUser
                {
                    Id = "276b2f75-655a-4ac9-802d-ed06323d5cc6",
                    FullName = "Sulthon Abdillah"
                }
            };

            var totalAnswers = exerciseTransaction.CorrectAnswer + exerciseTransaction.IncorrectAnswer;
            exerciseTransaction.Score = (int)Math.Round((double)exerciseTransaction.CorrectAnswer / totalAnswers * 100);


            var exerciseTransactionDto = new ExerciseTransactionDto
            {
                Id = exerciseTransaction.Id,
                StartDate = exerciseTransaction.StartDate,
                EndDate = exerciseTransaction.EndDate,
                CorrectAnswer = exerciseTransaction.CorrectAnswer,
                IncorrectAnswer = exerciseTransaction.IncorrectAnswer,
                Score = exerciseTransaction.Score,
                SubChapter = exerciseTransaction.SubChapter.Title,
                UserFullName = exerciseTransaction.User.FullName,
                HistoryAnswer = new List<HistoryAnswerDto>()
            };

            foreach (var historyAnswer in exerciseTransaction.HistoryAnswer)
            {
                var historyAnswerDto = new HistoryAnswerDto
                {
                    Id = historyAnswer.Id,
                    FkQuestionId = historyAnswer.FkQuestionId,
                    Question = historyAnswer.Question.QuestionText,
                    FkOptionId = historyAnswer.FkOptionId,
                    Option = historyAnswer.Option.OptionText,
                    IsAnswer = historyAnswer.Option.IsAnswer
                };
                exerciseTransactionDto.HistoryAnswer.Add(historyAnswerDto);
            }

            var response = new
            {
                Code = StatusCodes.Status201Created,
                Status = "Created",
                Message = "Berhasil mengerjakan latihan soal",
                Data = exerciseTransactionDto
            };

            A.CallTo(() => fakeMapper.Map<ExerciseTransaction, ExerciseTransactionDto>(exerciseTransaction)).Returns(exerciseTransactionDto);
            A.CallTo(() => fakeExerciseTransactionRepository.PostAsync(A<ExerciseTransaction>.Ignored)).Returns(Task.CompletedTask);
            A.CallTo(() => fakeExerciseTransactionRepository.GetAsync(exerciseTransaction.Id)).Returns(Task.FromResult(exerciseTransaction));

            // Act
            var result = await controller.CreateExerciseTransaction(exerciseTransaction);

            // Assert
            var okResult = Assert.IsType<CreatedResult>(result); // Change from OkObjectResult to CreatedResult
            okResult.Should().NotBeNull();

            var returnedResponse = okResult.Value;
            returnedResponse.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task PostAsync_ShouldReturnBadRequestWhenHistoryIsNull()
        {
            // Arrange
            var fakeExerciseTransactionRepository = A.Fake<IExerciseTransactionRepository>();
            var fakeMapper = A.Fake<IMapper>();

            var controller = new ExerciseTransactionController(fakeExerciseTransactionRepository, fakeMapper);

            var exerciseTransaction = new ExerciseTransaction
            {
                Id = 1,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMinutes(10),
                CorrectAnswer = 8,
                IncorrectAnswer = 2,
                FkSubChapterId = 1,
                FkUserId = "276b2f75-655a-4ac9-802d-ed06323d5cc6",
                HistoryAnswer = [], // Empty history answer
                SubChapter = new SubChapter
                {
                    Title = "Image Editing"
                },
                User = new ApplicationUser
                {
                    FullName = "Sulthon Abdillah"
                }
            };

            // Act
            var result = await controller.CreateExerciseTransaction(exerciseTransaction);

            var response = new
            {
                Code = StatusCodes.Status400BadRequest,
                Status = "Bad Request",
                Message = "History Answer tidak boleh kosong"
            };

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            badRequestResult.Should().NotBeNull();
            badRequestResult.Value.Should().BeEquivalentTo(response);
        }
    }
}
