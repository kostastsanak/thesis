import { Message, Pagging } from "src/app/core/models/core.models"

export interface ConsumeEnergyRequest {
    cityID: number
    city: string
    year: number
    month: number
    day: number
    groupByYear: number
    groupByMonth: number
    groupByCity: number
    limit: number
    page: number
    noPagging: number
  }
  
  export interface ConsumeEnergyResponse {
    timestamp: string
    status: string
    message: string
    messages: Message[]
    payload: ConsumeEnergyPayload
  }
  

  export interface ConsumeEnergyPayload {
    pagging: Pagging
    consumptions: Consumption[]
  }
  
  export interface Consumption {
    energy_mwh: number
    area: string
    date: string
  }
  