using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using DanClarkeBlog.Core.Data;

namespace DanClarkeBlog.Core.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20170507175624_AddUniqueConstraintOnTagName")]
    partial class AddUniqueConstraintOnTagName
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DanClarkeBlog.Core.Models.BlogPost", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Featured");

                    b.Property<string>("HtmlShortText");

                    b.Property<string>("HtmlText");

                    b.Property<DateTime?>("PublishDate");

                    b.Property<bool>("Published");

                    b.Property<string>("Route");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.ToTable("BlogPosts");
                });

            modelBuilder.Entity("DanClarkeBlog.Core.Models.BlogPostTag", b =>
                {
                    b.Property<Guid>("BlogPostId");

                    b.Property<Guid>("TagId");

                    b.HasKey("BlogPostId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("BlogPostTags");
                });

            modelBuilder.Entity("DanClarkeBlog.Core.Models.DropboxCursor", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Cursor");

                    b.HasKey("Id");

                    b.ToTable("DropboxCursors");
                });

            modelBuilder.Entity("DanClarkeBlog.Core.Models.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAlternateKey("Name")
                        .HasName("AlternateKey_TagName");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("DanClarkeBlog.Core.Models.BlogPostTag", b =>
                {
                    b.HasOne("DanClarkeBlog.Core.Models.BlogPost", "BlogPost")
                        .WithMany("BlogPostTags")
                        .HasForeignKey("BlogPostId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DanClarkeBlog.Core.Models.Tag", "Tag")
                        .WithMany("BlogPostTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
