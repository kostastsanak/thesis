import { Message, Pagging } from "src/app/core/models/core.models"

export interface TotalEnergyProductionRequest{
    fuelIDs: number[]
    fuel: string
    year: number
    month: number
    day: number
    groupByFuel: number
    groupByYear: number
    groupByMonth: number
    limit: number
    page: number
    noPagging: number
  }


export interface TotalEnergyProductionPayload {
    pagging: Pagging
    productions: Production[]
  }
  
  export interface TotalEnergyProductionResponse {
    timestamp: string
    status: string
    message: string
    messages: Message[]
    payload: TotalEnergyProductionPayload
  }
  export interface Production {
    energy_mwh: number
    date: string
    fuel: string
  }
  