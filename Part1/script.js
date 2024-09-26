document.addEventListener('DOMContentLoaded', () => {
    const dateInput = document.getElementById('entryDate');
    const descriptionInput = document.getElementById('entryDescription');
    const submitButton = document.getElementById('submitButton');

    // max date to today's date and min date to three days before today
    const today = new Date();
    const maxDate = today.toISOString().split('T')[0];
    const minDate = new Date(today.setDate(today.getDate() - 3)).toISOString().split('T')[0];

    dateInput.setAttribute('max', maxDate); // Max date set to today
    dateInput.setAttribute('min', minDate); // Min date set to 3 days before today

    // Enables the submit button when the description is 30 characters or more
    descriptionInput.addEventListener('input', () => {
        if (descriptionInput.value.length >= 30) {
            submitButton.disabled = false; // Enable button if description is valid
        } else {
            submitButton.disabled = true; // Disable button if description is too short
        }
    });

    // Form submission handler that prevents default submission and shows an alert
    document.getElementById('timeEntryForm').addEventListener('submit', (event) => {
        event.preventDefault();
        alert('Time entry submitted!');
    });
});
