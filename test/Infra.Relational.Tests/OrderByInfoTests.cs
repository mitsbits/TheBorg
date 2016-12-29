using System;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Xunit;

namespace Borg.Infra.Relational.Tests
{

    public class OrderByInfoTests
    {
        private List<Post> _db;


        public  OrderByInfoTests()
        {
            _db = new List<Post>();
        }
        [Fact]
        public void check_order_info_on_a_single_string_property()
        {
            PopulateDb();
            _db.AsQueryable().Apply(new OrderByInfo<Post>() { Ascending = true, Property = x => x.Title }).First().Title.ShouldBe("article");
            _db.AsQueryable().Apply(new OrderByInfo<Post>() { Ascending = false, Property = x => x.Title }).First().Title.ShouldBe("some title");
        }

        [Fact]
        public void check_order_info_on_a_single_integer_property()
        {
            PopulateDb();
            _db.AsQueryable().Apply(new OrderByInfo<Post>() { Ascending = true, Property = x => x.Id }).First().Id.ShouldBe(1);
            _db.AsQueryable().Apply(new OrderByInfo<Post>() { Ascending = false, Property = x => x.Id }).First().Id.ShouldBe(3);
        }

        [Fact]
        public void check_order_info_on_a_single_date_property()
        {
            PopulateDb();
            _db.AsQueryable().Apply(new OrderByInfo<Post>() { Ascending = true, Property = x => x.CreatedOn }).First().Id.ShouldBe(3);
            _db.AsQueryable().Apply(new OrderByInfo<Post>() { Ascending = false, Property = x => x.CreatedOn }).First().Id.ShouldBe(1);
        }

        [Fact]
        public void check_order_info_on_a_single_complex_property_on_integer_field()
        {
            PopulateDb();
            _db.AsQueryable().Apply(new OrderByInfo<Post>() { Ascending = true, Property = x => x.Author.Id }).First().Id.ShouldBe(3);
            _db.AsQueryable().Apply(new OrderByInfo<Post>() { Ascending = false, Property = x => x.Author.Id }).First().Id.ShouldBe(1);
        }

        [Fact]
        public void check_order_info_on_a_single_complex_property_on_string_field()
        {
            PopulateDb();
            _db.AsQueryable().Apply(new OrderByInfo<Post>() { Ascending = true, Property = x => x.Author.UserName }).First().Author.UserName.ShouldBe("none");
            _db.AsQueryable().Apply(new OrderByInfo<Post>() { Ascending = false, Property = x => x.Author.UserName }).First().Author.UserName.ShouldBe("test");
        }

        [Fact]
        public void check_order_info_on_a_collection_aggregate()
        {
            PopulateDb();
            _db.AsQueryable().Apply(new OrderByInfo<Post>() { Ascending = true, Property = x => x.Tags.Count }).First().Id.ShouldBe(3);
            _db.AsQueryable().Apply(new OrderByInfo<Post>() { Ascending = false, Property = x => x.Tags.Count }).First().Id.ShouldBe(2);
        }

        void PopulateDb()
        {
            _db.Clear();
            var tags = new Dictionary<string, Tag>()
            {
                { "Shop", new Tag("Shop") },
                { "TV", new Tag("TV") },
                { "Cinema", new Tag("Cinema") },
                { "Radio", new Tag("Radio") },
            };

            var users = new Dictionary<int, User>()
            {
                { 1, new User() {Id = 1, Email = "test@test.com", UserName = "test"} },
                { 2, new User() {Id = 2, Email = "some@were.com", UserName = "some"}},
                { 3, new User() {Id = 3, Email = "no@one.com", UserName = "none"}},
            };

            _db.Add(new Post()
            {
                Id = 1,
                Author = users[3],
                Body = "some text",
                CreatedOn = DateTimeOffset.UtcNow.AddMinutes(-5),
                Tags = new List<Tag>() { tags["TV"], tags["Radio"] },
                Title = "some title"
            });

            _db.Add(new Post()
            {
                Id = 2,
                Author = users[2],
                Body = "this is supposed to be the body of an article",
                CreatedOn = DateTimeOffset.UtcNow.AddMinutes(-10),
                Tags = new List<Tag>() { tags["Cinema"], tags["Shop"], tags["Radio"] },
                Title = "article"
            });

            _db.Add(new Post()
            {
                Id = 3,
                Author = users[1],
                Body = "Yes - you can mix both strongly typed queries and the dynamic string based queries together, which is quite nice and useful.",
                CreatedOn = DateTimeOffset.UtcNow.AddMinutes(-15),
                Tags = new List<Tag>() { tags["TV"] },
                Title = "mix both strongly"
            });

        }
    }

    internal class Tag
    {
        public Tag() { }
        public Tag(string value)
        {
            Value = value;
        }
        public string Value { get; set; }
    }

    internal class Post
    {
        public virtual ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.UtcNow;
        public string Title { get; set; }
        public string Body { get; set; }
        public int Id { get; set; }
        public User Author { get; set; }

    }

    internal class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
    }
}