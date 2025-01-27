namespace API.Extensions;


public static class DateTimeExtensions
{
    public static int CalculateAge(this DateOnly dob)
    {
        var currentDate = DateOnly.FromDateTime(DateTime.Today);
        var age = currentDate.Year - dob.Year;

        if (dob > currentDate.AddYears(-age)) age--;

        return age;
    }
}