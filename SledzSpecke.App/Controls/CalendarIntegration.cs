using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SledzSpecke.Core.Models.Domain;

namespace SledzSpecke.App.Controls
{
    public class CalendarIntegration
    {
        private readonly ICalendarService _calendarService;
        
        public CalendarIntegration(ICalendarService calendarService)
        {
            _calendarService = calendarService;
        }
        
        public async Task<bool> AddDutyToCalendarAsync(Duty duty)
        {
            try
            {
                var eventId = await _calendarService.AddEventAsync(
                    $"Dyżur: {duty.Location}",
                    duty.StartTime,
                    duty.EndTime,
                    $"Dyżur medyczny\nMiejsce: {duty.Location}\n{duty.Notes}",
                    true);
                
                return !string.IsNullOrEmpty(eventId);
            }
            catch (Exception ex)
            {
                // Obsługa błędów
                System.Diagnostics.Debug.WriteLine($"Error adding duty to calendar: {ex.Message}");
                return false;
            }
        }
        
        public async Task<bool> AddCourseToCalendarAsync(Course course)
        {
            try
            {
                if (!course.StartDate.HasValue || !course.EndDate.HasValue)
                    return false;
                
                var courseTitle = course.Definition?.Name ?? "Kurs specjalizacyjny";
                var eventId = await _calendarService.AddEventAsync(
                    courseTitle,
                    course.StartDate.Value,
                    course.EndDate.Value,
                    $"Kurs specjalizacyjny\nOrganizator: {course.Organizer}\nMiejsce: {course.Location}\n{course.Notes}",
                    true);
                
                return !string.IsNullOrEmpty(eventId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding course to calendar: {ex.Message}");
                return false;
            }
        }
        
        public async Task<bool> AddInternshipToCalendarAsync(Internship internship)
        {
            try
            {
                if (!internship.StartDate.HasValue || !internship.EndDate.HasValue)
                    return false;
                
                var internshipTitle = internship.Definition?.Name ?? "Staż specjalizacyjny";
                var eventId = await _calendarService.AddEventAsync(
                    internshipTitle,
                    internship.StartDate.Value,
                    internship.EndDate.Value,
                    $"Staż specjalizacyjny\nMiejsce: {internship.Location}\n{internship.Notes}",
                    true);
                
                return !string.IsNullOrEmpty(eventId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding internship to calendar: {ex.Message}");
                return false;
            }
        }
        
        public async Task<bool> RemoveFromCalendarAsync(string eventId)
        {
            try
            {
                return await _calendarService.RemoveEventAsync(eventId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error removing event from calendar: {ex.Message}");
                return false;
            }
        }
        
        public async Task<List<CalendarEvent>> GetAllEventsAsync(DateTime start, DateTime end)
        {
            try
            {
                return await _calendarService.GetEventsAsync(start, end);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting events from calendar: {ex.Message}");
                return new List<CalendarEvent>();
            }
        }
    }
    
    // Klasa do przechowywania informacji o wydarzeniach kalendarzowych
    public class CalendarEvent
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public bool IsAllDay { get; set; }
    }
    
    // Interfejs usługi kalendarzowej
    public interface ICalendarService
    {
        Task<string> AddEventAsync(string title, DateTime start, DateTime end, string description, bool isAllDay);
        Task<bool> RemoveEventAsync(string eventId);
        Task<List<CalendarEvent>> GetEventsAsync(DateTime start, DateTime end);
    }
}
