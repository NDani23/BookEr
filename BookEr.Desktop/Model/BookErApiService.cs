using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BookEr.DTO;

namespace BookEr.Desktop.Model
{
    public class BookErApiService
    {
        private readonly HttpClient _client;

        public BookErApiService(string baseAddress)
        {
            _client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress)
            };
        }

        public async Task<IEnumerable<BookDto>> LoadBooksAsync()
        {
            HttpResponseMessage response = await _client.GetAsync("api/Books/");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<IEnumerable<BookDto>>();
            }

            throw new NetworkException("Service returned response: " + response.StatusCode);
        }

        public async Task<IEnumerable<VolumeDto>> LoadVolumesAsync(int bookId)
        {
            HttpResponseMessage response = await _client.GetAsync($"api/Volumes/{bookId}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<IEnumerable<VolumeDto>>();
            }

            throw new NetworkException("Service returned response: " + response.StatusCode);
        }

        public async Task<IEnumerable<BorrowDto>> LoadBorrowsAsync(int libraryId)
        {
            HttpResponseMessage response = await _client.GetAsync($"api/Borrows/{libraryId}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<IEnumerable<BorrowDto>>();
            }

            throw new NetworkException("Service returned response: " + response.StatusCode);
        }

        public async Task AddVolumeToBookAsync(BookDto bookDto)
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync("api/Volumes/", bookDto);

            if (!response.IsSuccessStatusCode)
            {
                throw new NetworkException("Service returned response: " + response.StatusCode);
            }
        }

        public async Task<BookDto> AddBookAsync(BookDto bookDto, uint volumeCount)
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync("api/Books/", bookDto);

            if (!response.IsSuccessStatusCode)
            {
                throw new NetworkException("Service returned response: " + response.StatusCode);
            }

            return await response.Content.ReadAsAsync<BookDto>();
        }

        public async Task<bool> DeleteVolumeAsync(Int32 libraryId)
        {
            
            HttpResponseMessage response = await _client.DeleteAsync($"api/Volumes/{libraryId}");

            if (!response.IsSuccessStatusCode)
            {
                throw new NetworkException("Service returned response: " + response.StatusCode);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
            
        }

        public async Task ChangeVolumeAvailableAsync(VolumeDto volume)
        {

                volume.IsAvailable = volume.IsAvailable ? false : true;
                HttpResponseMessage response = await _client.PutAsJsonAsync($"api/Volumes/{volume.LibraryId}", volume);

                if (!response.IsSuccessStatusCode)
                {
                    throw new NetworkException("Service returned response: " + response.StatusCode);
                }
         

        }

        public async Task ChangeBorrowActiveStateAsync(BorrowDto borrow)
        {
            borrow.IsActive = borrow.IsActive ? false : true;
            HttpResponseMessage response = await _client.PutAsJsonAsync($"api/Borrows/{borrow.Id}", borrow);

            if (!response.IsSuccessStatusCode)
            {
                throw new NetworkException("Service returned response: " + response.StatusCode);
            }

        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            LoginDto user = new LoginDto
            {
                UserName = username,
                Password = password
            };

            HttpResponseMessage response = await _client.PostAsJsonAsync("api/Account/Login", user);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return false;
            }

            throw new NetworkException("Service returned response: " + response.StatusCode);
        }

        public async Task<ResponseDto> RegisterAsync(RegisterDto user)
        {
                HttpResponseMessage response = await _client.PostAsJsonAsync("api/Account/Register", user);

                ResponseDto responseDto = new ResponseDto();

                if (response.IsSuccessStatusCode)
                {
                    responseDto.Success = true;
                    return responseDto;
                }
                else
                {
                    responseDto.Success = false;
                    responseDto.ErrorMessage = await response.Content.ReadAsStringAsync();
                
                    return responseDto;
                }     

        }

        public async Task LogoutAsync()
        {
            HttpResponseMessage response = await _client.PostAsync("api/Account/Logout", null);

            if (response.IsSuccessStatusCode)
            {
                return;
            }


            throw new NetworkException("Service returned response: " + response);
        }
    }
}
