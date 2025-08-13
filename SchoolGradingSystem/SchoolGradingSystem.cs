using System;
using System.IO;
using System.Collections.Generic;


namespace SchoolGradeSystem
{
    public class Student
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public int Score { get; set; }


        public Student(int id, string fullname, int score)
        {
            Id = id;
            FullName = fullname;
            Score = score;
        }

        public string GetGrade()
        {
             if (Score >= 80 && Score <= 100)
                return "A"; 
             else if (Score >= 70 && Score <= 79)
                return "B";
             else if (Score >= 60 && Score <= 69)
                return "C";
             else if (Score >= 50 && Score <= 59)
                return "D";
             else
                return "F";
        }

    }

    public class InvalidScoreFormatException : Exception 
    { 
        public InvalidScoreFormatException() : base() { }

        public InvalidScoreFormatException(string message) : base(message) { }

        public InvalidScoreFormatException(string message, Exception inner ) : base(message, inner) { }
    }

    public class MissingFieldException : Exception
    {
        public MissingFieldException() : base() { }

        public MissingFieldException(string message) : base(message) { }

        public MissingFieldException(string message, Exception inner ) : base(message , inner) { }
    }

    public class StudentResultProcessor
    {
        public List<Student> ReadStudentsFromFile(string inputFilePath)
        {
            List<Student> students = new List<Student>();

            using (StreamReader reader = new StreamReader(inputFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(',');

                    // Check field count
                    if (parts.Length != 3)
                        throw new MissingFieldException($"Line '{line}' is missing required fields.");

                    // Parse ID
                    if (!int.TryParse(parts[0].Trim(), out int id))
                        throw new MissingFieldException($"Invalid ID format in line: '{line}'");

                    string fullName = parts[1].Trim();

                    // Parse Score
                    if (!int.TryParse(parts[2].Trim(), out int score))
                        throw new InvalidScoreFormatException($"Score '{parts[2].Trim()}' is not a valid integer.");

                    students.Add(new Student(id, fullName, score));
                }
            }

            return students;
        }

        public void WriteReportToFile(List<Student> students, string outputFilePath)
        {
            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                foreach (var student in students)
                {
                    writer.WriteLine($"{student.FullName} (ID: {student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}");
                }
            }
        }
    }

    class SchoolGradingSystem
    {
        static void Main(string[] args)
        {
            string inputFilePath = "students.txt";
            string outputFilePath = "report.txt";

            try
            {
                StudentResultProcessor processor = new StudentResultProcessor();

                List<Student> students = processor.ReadStudentsFromFile(inputFilePath);
                processor.WriteReportToFile(students, outputFilePath);

                Console.WriteLine("Report generated successfully!");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Error: Input file not found.");
            }
            catch (InvalidScoreFormatException ex)
            {
                Console.WriteLine($"Invalid score format: {ex.Message}");
            }
            catch (MissingFieldException ex)
            {
                Console.WriteLine($"Missing field: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}



   


