import apiClient from './axiosConfig';


export default {
  getTasks: async () => {
    const result = await apiClient.get(`/tasks`)
    return result.data;
  },

  addTask: async (name) => {
    try {
      const newTask = {
        name: name,
        isComplete: false,
      };
      const result = await apiClient.post(`/tasks`, newTask);
      return result.data;
    }
    catch (error) {
      console.error("Error adding task:", error);
      throw error; // משליך את השגיאה כדי לטפל בה ברמה גבוהה יותר במידת הצורך
    }
  },

  setCompleted: async (id, isComplete) => {
    try {
      const current = await apiClient.get(`/tasks/${id}`);
      console.log("current", current.data);
      current.data.isComplete = !(current.data.isComplete);
      console.log("current", current.data);
      const result = await apiClient.put(`/tasks/${id}`, current.data);
      return result.data;
    }
    catch (error) {
      console.error("Error update task:", error);
      // throw error; // משליך את השגיאה כדי לטפל בה ברמה גבוהה יותר במידת הצורך
    }
  },

  deleteTask: async (id) => {
    console.log('deleteTask')
    const result = await apiClient.delete(`/tasks/${id}`);
    return result;
  }
};
