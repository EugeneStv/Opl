using System;
using System.Collections.Generic;
using System.Linq;

abstract class Employee
{
    private string employeeId;
    private string lastName;
    private string firstName;
    private string phone;
}

class Administrator : Employee
{
    private string workTime;
    
    public Patient RegisterPatient(string fName, string lName, string phone, string ins, string bType, string alg)
    {
        var patient = new Patient(fName, lName, phone, ins);
        SetMedicalRecord(patient, bType, alg);
        return patient;
    }
    
    public Appointment CreateAppointment(Patient patient, Service sr)
    {
        var doctors = sr.GetDoctors();
        
        foreach (var dc in doctors)
        {
            var timeSlots = dc.GetSlots();
            if (timeSlots.Count > 0)
            {
                var slot = timeSlots[0];
                slot.Reserve();
                var ap = new Appointment(patient, dc, sr, slot);
                break;
            }
        }
        
        return ap;
    }
    
    public void ProcessPayment(Appointment appointment)
    {
        var payment = new Payment(appointment.GetServiceCost());
        payment.ProcessPayment();
    }
    
    private void SetMedicalRecord(Patient patient, string bType, string alg)
    {
        var r = new MedicalRecord(bType, alg);
        patient.SetRecord(r);
    }
}

class Doctor : Employee
{
    private string specialization;
    private string licenseNumber;
    private Room room;
    private List<TimeSlot> timeSlots = new List<TimeSlot>();
    
    public MedicalResult CreateResult(Patient patient, string type, string title, string description)
    {
        var mrec = patient.GetMedicalRecord();
        var mres = new MedicalResult(type, title, description);
        mrec.AddMedicalResult(mres);
        return mres;
    }
    
    public List<TimeSlot> GetSlots()
    {
        return timeSlots.Where(ts => ts.IsAvailable()).ToList();
    }
    
    public void AddTimeSlot(TimeSlot timeSlot)
    {
        timeSlots.Add(timeSlot);
    }
}

class Patient
{
    private string patientId;
    private string lastName;
    private string firstName;
    private string phone;
    private string insurance;
    private MedicalRecord medicalRecord;
    
    public Patient(string firstName, string lastName, string phone, string insurance)
    {
        this.patientId = $"PAT_{DateTime.Now.Ticks}";
        this.firstName = firstName;
        this.lastName = lastName;
        this.phone = phone;
        this.insurance = insurance;
    }
    
    public void UpdatePhone(string phone)
    {
        this.phone = phone;
    }
    
    public MedicalRecord GetMedicalRecord()
    {
        return medicalRecord;
    }
    
    public void SetRecord(MedicalRecord medicalRecord)
    {
        this.medicalRecord = medicalRecord;
    }
}

class MedicalRecord
{
    private string recordId;
    private string bloodType;
    private string allergies;
    private List<MedicalResult> medicalResults = new List<MedicalResult>();
    private List<Appointment> appointments = new List<Appointment>();
    
    public MedicalRecord(string bloodType, string allergies)
    {
        this.recordId = $"REC_{DateTime.Now.Ticks}";
        this.bloodType = bloodType;
        this.allergies = allergies;
    }
    
    public void AddAppointment(Appointment appointment)
    {
        appointments.Add(appointment);
    }
    
    public void AddMedicalResult(MedicalResult result)
    {
        medicalResults.Add(result);
    }
}

class Appointment
{
    private string appointmentId;
    private DateTime dateTime;
    private string status;
    private Patient patient;
    private Doctor doctor;
    private Service service;
	private Payment payment;
    
    public Appointment(Patient patient, Doctor doctor, Service service, TimeSlot timeSlot)
    {
        this.appointmentId = $"APT_{DateTime.Now.Ticks}";
        this.patient = patient;
        this.doctor = doctor;
        this.service = service;
        this.dateTime = new DateTime(timeSlot.GetDate().Year, timeSlot.GetDate().Month, timeSlot.GetDate().Day, 
                               timeSlot.GetStartTime().Hour, timeSlot.GetStartTime().Minute, 0);
        this.status = "Scheduled";
    }
    
    public void Cancel(string reason)
    {
        status = "Cancelled";
    }
    
    public void Complete(string notes)
    {
        status = "Completed";
    }
    
    public decimal GetServiceCost()
    {
        return service.GetCost();
    }
}

class Service
{
    private string serviceId;
    private string name;
    private decimal cost;
    private int duration;
    private List<Doctor> doctors = new List<Doctor>();
    
    public Service(string name, decimal cost, int duration)
    {
        this.serviceId = $"SVC_{DateTime.Now.Ticks}";
        this.name = name;
        this.cost = cost;
    }
    
    public void AddDoctor(Doctor doctor)
    {
        doctors.Add(doctor);
    }
    
    public List<Doctor> GetDoctors()
    {
        return doctors.Where(d => d.GetSlots().Count > 0).ToList();
    }
    
    public decimal GetCost()
    {
        return cost;
    }
}

class TimeSlot
{
    private string slotId;
    private DateOnly date;
    private TimeOnly startTime;
    private TimeOnly endTime;
    private bool isAvailable = true;
    
    public bool Reserve()
    {
        if (isAvailable)
        {
            isAvailable = false;
            return true;
        }
        return false;
    }
    
    public bool IsAvailable()
    {
        return isAvailable;
    }
    
    public DateOnly GetDate()
    {
        return date;
    }
    
    public TimeOnly GetStartTime()
    {
        return startTime;
    }
}

class MedicalResult
{
    private string resultId;
    private string type;
    private string title;
    private string description;
    private DateTime dateCreated;
    
    public MedicalResult(string type, string title, string description)
    {
        this.resultId = $"RES_{DateTime.Now.Ticks}";
        this.type = type;
        this.title = title;
        this.description = description;
        this.dateCreated = DateTime.Now;
    }
}

class Payment
{
    private string paymentId;
    private decimal amount;
    private DateTime date;
    private string status;
    
    public Payment(decimal amount)
    {
        this.paymentId = $"PAY_{DateTime.Now.Ticks}";
        this.amount = amount;
        this.date = DateTime.Now;
        this.status = "Pending";
    }
    
    public void ProcessPayment()
    {
        status = "Processed";
    }
}

class Room
{
    private string roomNumber;
    private string type;
}
















