using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace API.Helpers
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeader(this HttpResponse response, int currentPage
        , int itemPerPage, int totalItem, int totalPages){
            var paginationHeader = new PaginationHeader(currentPage, itemPerPage, totalItem, totalPages);
            
            var option = new JsonSerializerOptions{
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            
            response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader,option) );
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination" );    
        }
    }
}