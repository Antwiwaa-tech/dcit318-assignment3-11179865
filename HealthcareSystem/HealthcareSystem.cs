using System;
using System.Collections.Generic;

namespace healthcare_system
{
    // a. Generic Repository for Entity Management
    public class Repository<T> where T : class
    {
        private readonly List<T> items = new List<T>();

        public void Add(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            items.Add(item);
        }

        public List<T> GetAll()
        {
            return new List<T>(items);
        }

        public T GetById(Func<T, bool> predicate)
        {
            foreach (var item in items)
            {
                if (predicate(item))
                    return item;
            }
            return null;
        }

        public bool Remove(Func<T, bool> predicate)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (predicate(items[i]))
                {
                    items.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
    }

    // b. Patient class
    public class Patient
    {
        public int Id { get; }
        public string Name { get; }
        public int Age { get; }
        public string Gender { get; }

        public Patient(int id, string name, int age, string gender)
        {
            Id = id;
            Name = name;
            Age = age;
            Gender = gender;
        }

        public override string ToString()
        {
            return $"Patient {{ Id = {Id}, Name = {Name}, Age = {Age}, Gender = {Gender} }}";
        }
    }

    // c. Prescription class
    public class Prescription
    {
        public int Id { get; }
        public int PatientId { get; }
        public string MedicationName { get; }
        public DateTime DateIssued { get; }

        public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
        {
            Id = id;
            PatientId = patientId;
            MedicationName = medicationName;
            DateIssued = dateIssued;
        }

        public override string ToString()
        {
            return $"Prescription {{ Id = {Id}, PatientId = {PatientId}, Medication = {MedicationName}, DateIssued = {DateIssued:yyyy-MM-dd} }}";
        }
    }

    // g. HealthSystemApp
    public class HealthSystemApp
    {
        private Repository<Patient> _patientRepo = new Repository<Patient>();
        private Repository<Prescription> _prescriptionRepo = new Repository<Prescription>();
        private Dictionary<int, List<Prescription>> _prescriptionMap = new Dictionary<int, List<Prescription>>();

        public void SeedData()
        {
            _patientRepo.Add(new Patient(1, "Alice Johnson", 34, "Female"));
            _patientRepo.Add(new Patient(2, "Bob Smith", 45, "Male"));
            _patientRepo.Add(new Patient(3, "Carol Lee", 29, "Female"));

            _prescriptionRepo.Add(new Prescription(1, 1, "Amoxicillin", new DateTime(2025, 1, 10)));
            _prescriptionRepo.Add(new Prescription(2, 1, "Ibuprofen", new DateTime(2025, 3, 5)));
            _prescriptionRepo.Add(new Prescription(3, 2, "Lisinopril", new DateTime(2025, 2, 20)));
            _prescriptionRepo.Add(new Prescription(4, 3, "Metformin", new DateTime(2025, 4, 1)));
            _prescriptionRepo.Add(new Prescription(5, 2, "Atorvastatin", new DateTime(2025, 5, 12)));
        }

        public void BuildPrescriptionMap()
        {
            _prescriptionMap.Clear();

            var allPrescriptions = _prescriptionRepo.GetAll();
            foreach (var prescription in allPrescriptions)
            {
                if (!_prescriptionMap.ContainsKey(prescription.PatientId))
                {
                    _prescriptionMap[prescription.PatientId] = new List<Prescription>();
                }
                _prescriptionMap[prescription.PatientId].Add(prescription);
            }
        }

        public void PrintAllPatients()
        {
            Console.WriteLine("All Patients:");
            foreach (var patient in _patientRepo.GetAll())
            {
                Console.WriteLine(patient);
            }
        }

        public void PrintPrescriptionsForPatient(int patientId)
        {
            Console.WriteLine($"\nPrescriptions for PatientId {patientId}:");
            if (_prescriptionMap.ContainsKey(patientId) && _prescriptionMap[patientId].Count > 0)
            {
                foreach (var pres in _prescriptionMap[patientId])
                {
                    Console.WriteLine(pres);
                }
            }
            else
            {
                Console.WriteLine("No prescriptions found for this patient.");
            }
        }

        public List<Prescription> GetPrescriptionsByPatientId(int patientId)
        {
            if (_prescriptionMap.ContainsKey(patientId))
            {
                return new List<Prescription>(_prescriptionMap[patientId]);
            }
            return new List<Prescription>();
        }

        public static void Main(string[] args)
        {
            var app = new HealthSystemApp();

            app.SeedData();
            app.BuildPrescriptionMap();
            app.PrintAllPatients();

            int selectedPatientId = 2;
            app.PrintPrescriptionsForPatient(selectedPatientId);
        }
    }
}
