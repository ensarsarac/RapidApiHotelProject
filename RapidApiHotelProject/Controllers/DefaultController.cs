using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RapidApiHotelProject.Models;

namespace RapidApiHotelProject.Controllers
{
    public class DefaultController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SearchHotel(SearchHotelViewModel model)
        {
            TempData["checkin"] = model.CheckIn.ToString("yyyy-MM-dd");
            TempData["checkout"] = model.CheckOut.ToString("yyyy-MM-dd");

            string destid =await GetHotelDestId(model.City);
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://booking-com15.p.rapidapi.com/api/v1/hotels/searchHotels?dest_id={destid}&search_type=CITY&arrival_date={model.CheckIn.ToString("yyyy-MM-dd")}&departure_date={model.CheckOut.ToString("yyyy-MM-dd")}&adults={model.AdultCount}&children_age=0%2C17&room_qty={model.RoomCount}&page_number=1&languagecode=en-us&currency_code=USD"),
                Headers =
    {
        { "X-RapidAPI-Key", "a506d0b55amshcfb29f2e4989817p174e30jsn4ef35ad88098" },
        { "X-RapidAPI-Host", "booking-com15.p.rapidapi.com" },
    },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<GetHotelListViewModel>(body);
                return View(values.data.hotels.ToList());
            }
        }

        public async Task<IActionResult> HotelDetails(string name,string photo,string desc,string price)
        {
            var value = new HotelDetailViewModel
            {
                desc = desc,
                name = name,
                price = price,
                photo=photo
            };
            return View(value);
        }
        public async Task<string> GetHotelDestId(string city)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://booking-com15.p.rapidapi.com/api/v1/hotels/searchDestination?query={city}"),
                Headers =
    {
        { "X-RapidAPI-Key", "a506d0b55amshcfb29f2e4989817p174e30jsn4ef35ad88098" },
        { "X-RapidAPI-Host", "booking-com15.p.rapidapi.com" },
    },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<GetDestIdViewModel>(body);
                return values.data[0].dest_id.ToString();
            }
        }
    }
}
