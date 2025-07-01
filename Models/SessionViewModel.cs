using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASSNlearningManagementSystem.Models
{
    public class SessionViewModel
    {
        // ✅ Holds the form data for creating/updating a session
        public Session NewSession { get; set; } = new Session();

        // ✅ Displays all sessions in the listing table
        public List<Session> SessionsList { get; set; } = new List<Session>();

        // ✅ Dropdowns for Trainer, Course, Syllabus, Topic
        public List<KeyValuePair<int, string>> Trainers { get; set; } = new List<KeyValuePair<int, string>>();
        public List<KeyValuePair<int, string>> Courses { get; set; } = new List<KeyValuePair<int, string>>();
        public List<KeyValuePair<int, string>> Syllabuses { get; set; } = new List<KeyValuePair<int, string>>();
        public List<KeyValuePair<int, string>> Topics { get; set; } = new List<KeyValuePair<int, string>>();
    }
}