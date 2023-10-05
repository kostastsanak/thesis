import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { TotalEnergyProductionRequest, TotalEnergyProductionResponse } from '../model/totalEnergyProduction.model';

const BASE_URL = 'https://localhost:7174/api/';

@Injectable({
  providedIn: 'root'
})
export class TotalEnergyProductionService {

  totalEnergyProductionRequestSubject$ = new BehaviorSubject<TotalEnergyProductionRequest | null>(null);

  constructor(private http: HttpClient) {}

  setTotalEnergyProductionRequest(req: TotalEnergyProductionRequest) {
    this.totalEnergyProductionRequestSubject$.next(req);
  }

  totalEnergyProductionSearch(req: TotalEnergyProductionRequest){
      return this.http.post<TotalEnergyProductionResponse>(
        `${BASE_URL}gov/electricity/production/search`,
        req
      );
  }
  totalEnergyProductionUpdate(){
    return this.http.put<any>(
      `${BASE_URL}gov/electricity/production/update`,null
    );
}

}
