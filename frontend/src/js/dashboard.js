// Get canvas context
const ctx = document.getElementById("sensorChart").getContext("2d");

// Create time-series chart
new Chart(ctx, {
    type: "line",
    data: {
        labels: sensorData.labels,
        datasets: [
            {
                label: "Temperature (°C)",
                data: sensorData.temperature,
                fill: false,
                borderWidth: 2,
                tension: 0.4
            },
            {
                label: "Vibration (mm/s)",
                data: sensorData.vibration,
                fill: false,
                borderWidth: 2,
                tension: 0.4
            }
        ]
    },
    options: {
        responsive: true,
        plugins: {
            legend: {
                position: "top"
            },
            title: {
                display: true,
                text: "Machine Sensor Time-Series Data"
            }
        },
        scales: {
            y: {
                beginAtZero: true
            }
        }
    }
});
