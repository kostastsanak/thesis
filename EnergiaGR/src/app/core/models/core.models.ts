export interface Message {
    messageType: number
    messageCode: string
    content: string
  }
  

  export interface Pagging {
    currentPage: number
    totalEntries: number
    entriesPerPage: number
    totalPages: number
    entriesStart: number
  }