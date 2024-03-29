﻿using GameApp.Data;
using GameApp.Data.Models;
using GameApp.Data.Repositories;
using GameApp.Services;
using GameApp.Tests.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GameApp.Tests.Services
{
    public class CommentsServiceTests
    {
        private List<Comment> GetDummyData()
        {
            var comments= new List<Comment>();
            var user1 = new User {
                Id="1",
                UserName="1"
            };
            var user2 = new User
            {
                Id = "2",
                UserName = "2"
            };
            for (int i = 0; i < 16; i++)
            {
                var comment = new Comment 
                {
                    Id=i.ToString(),
                    Content="Content"+i.ToString(),
                    
                };
                if (i%2==0)
                {
                    comment.GameId = 1;
                    comment.User = user1;
                }
                else
                {
                    comment.GameId = 2;
                    comment.User = user2;
                }
                if (i%3==0&&i!=0)
                {
                    comment.CommentedOnId = (i - 1).ToString();
                }
                comments.Add(comment);
            }

            return comments;
        }
        private async Task SeedData(GameAppDbContext context)
        {
            context.Comments.AddRange(GetDummyData());
            await context.SaveChangesAsync();
        }
        [Theory]
        [InlineData(0,1)]
        [InlineData(999,0)]

        public async Task TestAllCommentsShouldReturnAllComments(int page,int gameId)
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var repo = new Repository<Comment>(context);
            var commentsService = new CommentsService(repo);

            var result =(await commentsService.LoadComments(page, gameId)).ToList();

            var actualData = repo.All()
                .Where(c => c.GameId == gameId && c.CommentedOnId == null)
                .Skip(page * 10)
                .Take(10)
                .ToList();

            for (int i = 0; i < result.Count(); i++)
            {
                Assert.Equal(result[i].Contents, actualData[i].Content);
                if (actualData[i].Comments.Count() > 0)
                {
                    Assert.True(result[i].HasComments);
                }
                else
                {
                    Assert.False(result[i].HasComments);
                } 
                Assert.Equal(result[i].CommentId, actualData[i].Id);
            }

        }
        [Theory]
        [InlineData("2")]
        [InlineData("5")]
        [InlineData(null)]
        public async Task TestLoadReplies(string commentId)
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var repo = new Repository<Comment>(context);
            var commentsService = new CommentsService(repo);

            var result =(await commentsService.LoadReplies(commentId)).ToList();
            var actualData = repo.All().Where(c => c.CommentedOnId == commentId).ToList();

            for (int i = 0; i < result.Count(); i++)
            {
                Assert.Equal(result[i].Content, actualData[i].Content);
                if (actualData[i].Comments.Count() > 0)
                {
                    Assert.True(result[i].HasComments);
                }
                else
                {
                    Assert.False(result[i].HasComments);
                }
                Assert.Equal(result[i].CommentId, actualData[i].Id);
            }
        }
        [Fact]
        public async Task TestLoadRepliesWithImposibleValues()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var repo = new Repository<Comment>(context);
            var commentsService = new CommentsService(repo);

            var result = (await commentsService.LoadReplies("")).ToList();
            
            Assert.True(result.Count == 0);
        }

        [Fact]
        public async Task TestCreateComment()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var repo = new Repository<Comment>(context);
            var game = new Game
            {
                Id = 30,
                Name = "GameNew",
                Description = "Description",
                Price = 30,
                ImageUrl = "User.png",
                ReleaseDate = DateTime.MinValue,
            };
            await context.Games.AddAsync(game);
            await context.SaveChangesAsync();

            var commentsService = new CommentsService(repo);
            
            await commentsService.Create(30,"smt","1");
            var newComment = repo.All().Last();

            var actualComment = new Comment 
            {
                Content="smt",
                GameId=30,
                UserId="1",

            };
            Assert.Equal(newComment.Content, actualComment.Content);
            Assert.Equal(newComment.GameId, actualComment.GameId);
            Assert.Equal(newComment.UserId, actualComment.UserId);
        }
        [Fact]
        public async Task TestCreateReply()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var repo = new Repository<Comment>(context);
            var game = new Game
            {
                Id = 30,
                Name = "GameNew",
                Description = "Description",
                Price = 30,
                ImageUrl = "User.png",
                ReleaseDate = DateTime.MinValue,
            };
            await context.Games.AddAsync(game);
            await context.SaveChangesAsync();
            var commentsService = new CommentsService(repo);

            await commentsService.CreateReply(30,"smt","1","3");
            var newComment = repo.All().Last();
            var actualReply = new Comment 
            { 
                Content="smt",
                GameId = 30,
                UserId = "1",
                CommentedOnId="3",
            };

            Assert.Equal(newComment.Content, actualReply.Content);
            Assert.Equal(newComment.GameId, actualReply.GameId);
            Assert.Equal(newComment.UserId, actualReply.UserId);
            Assert.Equal(newComment.CommentedOnId, actualReply.CommentedOnId);
        }
        [Fact]
        public async Task TestCommentExist()
        {
            var context = GameAppDbContextFactory.InitializeContext();
            await SeedData(context);
            var repo = new Repository<Comment>(context);
            var commentsService = new CommentsService(repo);

            Assert.True(await commentsService.CommentExist("1"));
            Assert.False(await commentsService.CommentExist("NoComment"));
        }
    }
}
