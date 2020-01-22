using Shop.Application.Cart;
using Shop.Application.Oders;
using Shop.Application.Payment;
using Shop.Application.Products;
using Shop.Application.Services.FavouriteLists;
using Shop.Application.Services.Quotes;
using Shop.Application.Services.PDF;
using Shop.Application.Services.Emails;
using Shop.Application.Services.Cart;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceRegister
    {
        public static IServiceCollection RegisterApplicationServices(this IServiceCollection @this)
        {
            @this.AddTransient<GetProducts>();

            @this.AddTransient<AddProductToList>();
            @this.AddTransient<CreateList>();
            @this.AddTransient<DeleteList>();
            @this.AddTransient<GetLists>();
            @this.AddTransient<GetListsFull>();
            @this.AddTransient<UpdateList>();

            @this.AddSingleton<OneOffPayment>();

            @this.AddTransient<GetOrder>();
            @this.AddTransient<CreateOrder>();

            @this.AddTransient<AddToCart>();
            @this.AddTransient<GetCart>();
            @this.AddTransient<SetCartAddress>();

            @this.AddTransient<QuotesService>();
            @this.AddTransient<PDFService>();
            @this.AddTransient<EmailSender>();

            return @this;
        }
    }
}
