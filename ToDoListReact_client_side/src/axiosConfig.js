import axios from 'axios';

// יצירת אינסטנס של Axios עם הגדרות ברירת מחדל
const apiClient = axios.create({
  // baseURL: "http://localhost:5284", // כתובת ה-API הבסיסית
  baseURL: "https://todolist-server-8pzx.onrender.com", // כתובת ה-API הבסיסית
  // baseURL: process.env.REACT_APP_API_URL, // כתובת ה-API הבסיסית

  timeout: 10000, // זמן מקסימלי לבקשה (לא חובה)
  headers: {
    'Content-Type': 'application/json', // סוג תוכן ברירת מחדל
  },
});

// הוספת Interceptor לשגיאות
apiClient.interceptors.response.use(
  (response) => {
    // אם התשובה תקינה, מחזירים אותה
    return response;
  },
  (error) => {
    // כאן נתפוס את השגיאות ונרשום ללוג
    console.error('API error:', error.response || error.message);
    // אפשר גם להוסיף לוגים נוספים על פי הצורך
    // לדוג' - רשום שגיאות למערכת ניהול או שלח לאנליטיקס

    // אם השגיאה מכילה response, נתפוס אותה, אחרת נשלח את הודעת השגיאה הכללית
    if (error.response) {
      console.error('Response error:', error.response.data);
    } else if (error.request) {
      console.error('Request error:', error.request);
    } else {
      console.error('Error message:', error.message);
    }

    // מחזירים את השגיאה כך שהיא תוכל להתפשט הלאה (אם נרצה לטפל בה בצד הלקוח)
    return Promise.reject(error);
  }
);

export default apiClient;
