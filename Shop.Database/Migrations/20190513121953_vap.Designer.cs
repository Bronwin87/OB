﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Shop.Database;

namespace Shop.Database.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20190513121953_vap")]
    partial class vap
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Shop.Domain.Models.Accounts.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AddressId");

                    b.Property<string>("CompanyName");

                    b.Property<string>("Email");

                    b.Property<string>("PhoneNumber");

                    b.Property<string>("RegistrationNumber");

                    b.Property<bool>("TermAccount");

                    b.Property<bool>("ThirtyDayTermApproved");

                    b.Property<string>("VatNumber");

                    b.HasKey("Id");

                    b.HasIndex("AddressId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("Shop.Domain.Models.Accounts.AccountUser", b =>
                {
                    b.Property<int>("AccountId");

                    b.Property<string>("UserId");

                    b.Property<bool>("Active");

                    b.HasKey("AccountId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("AccountUsers");
                });

            modelBuilder.Entity("Shop.Domain.Models.Accounts.Location", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccountId");

                    b.Property<int>("AddressId");

                    b.Property<string>("AuthorizerId");

                    b.Property<string>("CompanyName");

                    b.Property<string>("CostCenter");

                    b.Property<string>("PhoneNumber");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("AddressId");

                    b.HasIndex("AuthorizerId");

                    b.HasIndex("UserId");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("Shop.Domain.Models.Accounts.Reference", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccountId");

                    b.Property<string>("CompanyName");

                    b.Property<string>("ContactName");

                    b.Property<string>("Email");

                    b.Property<string>("Telephone");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("References");
                });

            modelBuilder.Entity("Shop.Domain.Models.Address", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Address1");

                    b.Property<string>("Address2");

                    b.Property<string>("ApplicationUserId");

                    b.Property<string>("City");

                    b.Property<string>("Country");

                    b.Property<string>("PostCode");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("Shop.Domain.Models.Cart.Cart", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AddressId");

                    b.Property<string>("OrderReference");

                    b.Property<string>("SessionId");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("AddressId");

                    b.HasIndex("UserId");

                    b.ToTable("Carts");
                });

            modelBuilder.Entity("Shop.Domain.Models.Cart.CartProduct", b =>
                {
                    b.Property<string>("ProductId");

                    b.Property<int>("CartId");

                    b.Property<int>("Qty");

                    b.HasKey("ProductId", "CartId");

                    b.HasIndex("CartId");

                    b.ToTable("CartProducts");
                });

            modelBuilder.Entity("Shop.Domain.Models.Orders.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AccountId");

                    b.Property<int>("AddressId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("OrderNumber");

                    b.Property<string>("OrderRef");

                    b.Property<string>("PaymentReference");

                    b.Property<int>("Status");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("AddressId");

                    b.HasIndex("UserId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("Shop.Domain.Models.Orders.OrderProduct", b =>
                {
                    b.Property<string>("ProductId");

                    b.Property<int>("OrderId");

                    b.Property<int>("Qty");

                    b.HasKey("ProductId", "OrderId");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderProducts");
                });

            modelBuilder.Entity("Shop.Domain.Models.PendingOrders.PendingOrder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AccountId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("OrderNumber");

                    b.Property<int?>("QuoteId");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("UserId");

                    b.ToTable("PendingOrders");
                });

            modelBuilder.Entity("Shop.Domain.Models.PendingOrders.PendingOrderProduct", b =>
                {
                    b.Property<int>("PendingOrderId");

                    b.Property<string>("ProductId");

                    b.Property<int>("Id");

                    b.Property<int>("Qty");

                    b.HasKey("PendingOrderId", "ProductId");

                    b.HasAlternateKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("PendingOrderProducts");
                });

            modelBuilder.Entity("Shop.Domain.Models.Products.Discount", b =>
                {
                    b.Property<int>("AccountId");

                    b.Property<string>("ProductId");

                    b.Property<int>("DiscountPercent");

                    b.Property<int>("DiscountType");

                    b.Property<int>("DiscountValue");

                    b.HasKey("AccountId", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("Discounts");
                });

            modelBuilder.Entity("Shop.Domain.Models.Products.MainCategory", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CamelName");

                    b.Property<string>("Name");

                    b.Property<int>("Order");

                    b.HasKey("Id");

                    b.ToTable("MainCategories");
                });

            modelBuilder.Entity("Shop.Domain.Models.Products.Product", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BarCode");

                    b.Property<string>("Brand");

                    b.Property<string>("Colour");

                    b.Property<decimal>("CostPrice");

                    b.Property<string>("Description");

                    b.Property<string>("ExternalId");

                    b.Property<bool?>("ImageUpdated");

                    b.Property<string>("ImageUrl");

                    b.Property<string>("MainCategoryId");

                    b.Property<string>("Name");

                    b.Property<bool?>("NoDiscount");

                    b.Property<bool?>("OutOfStock");

                    b.Property<decimal?>("Price");

                    b.Property<bool>("Published");

                    b.Property<string>("SearchString");

                    b.Property<string>("SubCategoryId");

                    b.Property<string>("TertiaryCategoryId");

                    b.Property<string>("Unit");

                    b.Property<bool?>("ValueAddedProduct");

                    b.HasKey("Id");

                    b.HasIndex("MainCategoryId");

                    b.HasIndex("SubCategoryId");

                    b.HasIndex("TertiaryCategoryId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("Shop.Domain.Models.Products.SubCategory", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ImageUrl");

                    b.Property<string>("MainCategoryId");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("MainCategoryId");

                    b.ToTable("SubCategories");
                });

            modelBuilder.Entity("Shop.Domain.Models.Products.TertiaryCategory", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ImageUrl");

                    b.Property<string>("Name");

                    b.Property<string>("SubCategoryId");

                    b.HasKey("Id");

                    b.HasIndex("SubCategoryId");

                    b.ToTable("TertiaryCategories");
                });

            modelBuilder.Entity("Shop.Domain.Models.Quotes.Quote", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AccountId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("Name");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("UserId");

                    b.ToTable("Quotes");
                });

            modelBuilder.Entity("Shop.Domain.Models.Quotes.QuoteProduct", b =>
                {
                    b.Property<int>("QuoteId");

                    b.Property<string>("ProductId");

                    b.Property<int>("Id");

                    b.Property<int>("Qty");

                    b.HasKey("QuoteId", "ProductId");

                    b.HasAlternateKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("QuoteProducts");
                });

            modelBuilder.Entity("Shop.Domain.Models.Users.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirstName");

                    b.Property<string>("JobTitle");

                    b.Property<string>("LastName");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<int>("Type");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Shop.Domain.Models.Users.FavouriteList", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("FavouriteLists");
                });

            modelBuilder.Entity("Shop.Domain.Models.Users.FavouriteListProduct", b =>
                {
                    b.Property<int>("FavouriteListId");

                    b.Property<string>("ProductId");

                    b.Property<int>("Qty");

                    b.HasKey("FavouriteListId", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("FavouriteListProducts");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Shop.Domain.Models.Users.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Shop.Domain.Models.Users.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Shop.Domain.Models.Users.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Shop.Domain.Models.Users.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Shop.Domain.Models.Accounts.Account", b =>
                {
                    b.HasOne("Shop.Domain.Models.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId");
                });

            modelBuilder.Entity("Shop.Domain.Models.Accounts.AccountUser", b =>
                {
                    b.HasOne("Shop.Domain.Models.Accounts.Account", "Account")
                        .WithMany("AccountUsers")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Shop.Domain.Models.Users.ApplicationUser", "User")
                        .WithMany("Accounts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Shop.Domain.Models.Accounts.Location", b =>
                {
                    b.HasOne("Shop.Domain.Models.Accounts.Account", "Account")
                        .WithMany("Locations")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Shop.Domain.Models.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Shop.Domain.Models.Users.ApplicationUser", "Authorizer")
                        .WithMany()
                        .HasForeignKey("AuthorizerId");

                    b.HasOne("Shop.Domain.Models.Users.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Shop.Domain.Models.Accounts.Reference", b =>
                {
                    b.HasOne("Shop.Domain.Models.Accounts.Account", "Account")
                        .WithMany("References")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Shop.Domain.Models.Address", b =>
                {
                    b.HasOne("Shop.Domain.Models.Users.ApplicationUser")
                        .WithMany("Addresses")
                        .HasForeignKey("ApplicationUserId");
                });

            modelBuilder.Entity("Shop.Domain.Models.Cart.Cart", b =>
                {
                    b.HasOne("Shop.Domain.Models.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId");

                    b.HasOne("Shop.Domain.Models.Users.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Shop.Domain.Models.Cart.CartProduct", b =>
                {
                    b.HasOne("Shop.Domain.Models.Cart.Cart", "Cart")
                        .WithMany("Products")
                        .HasForeignKey("CartId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Shop.Domain.Models.Products.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Shop.Domain.Models.Orders.Order", b =>
                {
                    b.HasOne("Shop.Domain.Models.Accounts.Account", "Account")
                        .WithMany("Orders")
                        .HasForeignKey("AccountId");

                    b.HasOne("Shop.Domain.Models.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Shop.Domain.Models.Users.ApplicationUser", "User")
                        .WithMany("Orders")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Shop.Domain.Models.Orders.OrderProduct", b =>
                {
                    b.HasOne("Shop.Domain.Models.Orders.Order", "Order")
                        .WithMany("OrderProducts")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Shop.Domain.Models.Products.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Shop.Domain.Models.PendingOrders.PendingOrder", b =>
                {
                    b.HasOne("Shop.Domain.Models.Accounts.Account", "Account")
                        .WithMany("PendingOrders")
                        .HasForeignKey("AccountId");

                    b.HasOne("Shop.Domain.Models.Users.ApplicationUser", "User")
                        .WithMany("PendingOrders")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Shop.Domain.Models.PendingOrders.PendingOrderProduct", b =>
                {
                    b.HasOne("Shop.Domain.Models.PendingOrders.PendingOrder", "PendingOrder")
                        .WithMany("PendingOrderProducts")
                        .HasForeignKey("PendingOrderId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Shop.Domain.Models.Products.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Shop.Domain.Models.Products.Discount", b =>
                {
                    b.HasOne("Shop.Domain.Models.Accounts.Account", "Account")
                        .WithMany("Discounts")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Shop.Domain.Models.Products.Product", "Product")
                        .WithMany("Discounts")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Shop.Domain.Models.Products.Product", b =>
                {
                    b.HasOne("Shop.Domain.Models.Products.MainCategory", "MainCategory")
                        .WithMany("Products")
                        .HasForeignKey("MainCategoryId");

                    b.HasOne("Shop.Domain.Models.Products.SubCategory", "SubCategory")
                        .WithMany("Products")
                        .HasForeignKey("SubCategoryId");

                    b.HasOne("Shop.Domain.Models.Products.TertiaryCategory", "TertiaryCategory")
                        .WithMany("Products")
                        .HasForeignKey("TertiaryCategoryId");
                });

            modelBuilder.Entity("Shop.Domain.Models.Products.SubCategory", b =>
                {
                    b.HasOne("Shop.Domain.Models.Products.MainCategory", "MainCategory")
                        .WithMany("SubCategories")
                        .HasForeignKey("MainCategoryId");
                });

            modelBuilder.Entity("Shop.Domain.Models.Products.TertiaryCategory", b =>
                {
                    b.HasOne("Shop.Domain.Models.Products.SubCategory", "SubCategory")
                        .WithMany("TertiaryCategories")
                        .HasForeignKey("SubCategoryId");
                });

            modelBuilder.Entity("Shop.Domain.Models.Quotes.Quote", b =>
                {
                    b.HasOne("Shop.Domain.Models.Accounts.Account", "Account")
                        .WithMany("Quotes")
                        .HasForeignKey("AccountId");

                    b.HasOne("Shop.Domain.Models.Users.ApplicationUser", "User")
                        .WithMany("Quotes")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Shop.Domain.Models.Quotes.QuoteProduct", b =>
                {
                    b.HasOne("Shop.Domain.Models.Products.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Shop.Domain.Models.Quotes.Quote", "Quote")
                        .WithMany("QuoteProducts")
                        .HasForeignKey("QuoteId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Shop.Domain.Models.Users.FavouriteList", b =>
                {
                    b.HasOne("Shop.Domain.Models.Users.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Shop.Domain.Models.Users.FavouriteListProduct", b =>
                {
                    b.HasOne("Shop.Domain.Models.Users.FavouriteList", "FavouriteList")
                        .WithMany("FavouriteProducts")
                        .HasForeignKey("FavouriteListId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Shop.Domain.Models.Products.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
