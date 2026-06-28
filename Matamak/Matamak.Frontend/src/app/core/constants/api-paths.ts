export const API_PATHS = {
  auth: {
    register: '/api/v1/auth/register',
    staffRegister: '/api/v1/auth/manger&cashierRegister',
    activateAccount: (email: string) => `/api/v1/auth/activeAccount/${encodeURIComponent(email)}`,
    login: '/api/v1/auth/login',
    refreshToken: '/api/v1/auth/refreshToken',
    editAccount: (username: string) => `/api/v1/auth/EditAccount/${encodeURIComponent(username)}`,
    changePassword: (username: string) => `/api/v1/auth/ChangePassword/${encodeURIComponent(username)}`,
    deleteMyAccount: (username: string) => `/api/v1/auth/DeleteMyAccount/${encodeURIComponent(username)}`,
    deleteAnyAccount: (username: string) => `/api/v1/auth/DeleteAnyAccount/${encodeURIComponent(username)}`,
    forgotPassword: '/api/v1/auth/ForgotPassword',
    verifyForgotPasswordCode: (email: string) =>
      `/api/v1/auth/VerifyForgetPasswordCode/${encodeURIComponent(email)}`,
    resetPassword: (email: string) => `/api/v1/auth/ResetPassword/${encodeURIComponent(email)}`,
    getAllAdmins: '/api/v1/auth/GetAllAdmins',
    getAllCashiers: '/api/v1/auth/GetAllCashiers',
    getAllCustomers: '/api/v1/auth/GetAllCustomers',
    getCustomerByUsername: (username: string) =>
      `/api/v1/auth/GetCustomerByUsername/${encodeURIComponent(username)}`,
    logout: '/api/v1/auth/logout',
    orderHistory: (username: string) => `/api/v1/auth/order-history/${encodeURIComponent(username)}`
  },
  catalog: {
    categories: '/api/v1/categories',
    addCategory: '/api/v1/categories/addCategory',
    removeCategory: '/api/v1/categories/removeCategory',
    editCategory: '/api/v1/categories/editCategory',
    countries: '/api/v1/countries',
    addCountry: '/api/v1/countries/addCountry',
    removeCountry: '/api/v1/countries/removeCountry',
    editCountry: '/api/v1/countries/editCountry',
    items: '/api/v1/items',
    itemById: (id: number) => `/api/v1/items/${id}`,
    addItem: '/api/v1/items/addItem',
    updateItem: (id: number) => `/api/v1/items/updateItem/${id}`,
    removeItem: (id: number) => `/api/v1/items/removeItem/${id}`,
    search: '/api/v1/items/search',
    sortItems: '/api/v1/items/sortItems',
    reviewsByItem: (itemId: number) => `/api/v1/reviews/item/${itemId}`
  },
  orders: {
    createDelivery: '/api/DeliveryOrder/addDelveryOrder',
    getDeliveryOrders: '/api/DeliveryOrder/getDeliveryOrders',
    handOrderToDriver: (id: number) => `/api/DeliveryOrder/handOrderToDriver/${id}`,
    handOrderToCustomer: (id: number) => `/api/DeliveryOrder/handOrderToCustomer/${id}`,
    cancelDeliveryOrder: (id: number) => `/api/DeliveryOrder/cancelDeliveryOrder/${id}`,
    
    createTakeaway: '/api/TakeAwayOrder/addTakeAwayOrder',
    getTakeawayOrders: '/api/TakeAwayOrder/getAllTakeAwayOrders',
    removeTakeawayOrder: (id: number) => `/api/TakeAwayOrder/removeTakeAwayOrder/${id}`,
    changeTakeawayStatus: (id: number) => `/api/TakeAwayOrder/changeTakeawayOrderStatus/${id}`,

    getDineinOrders: '/api/DineinOrder/getAllDineinOrders',
    createDinein: '/api/DineinOrder/addDineinOrder',
    changeDineinStatus: (id: number) => `/api/DineinOrder/ChangeDineinOrderStatus/${id}`,
    removeDineinOrder: (id: number) => `/api/DineinOrder/removeDineinOrder/${id}`
  },
  inventory: {
    items: '/api/v1/inventory',
    lowStock: '/api/v1/inventory/low-stock'
  },
  reports: {
    sales: '/api/v1/reports/sales'
  }
} as const;
