import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ConsumeEnergyRequest, ConsumeEnergyResponse } from '../model/consumeEnergy.model';
import { BehaviorSubject } from 'rxjs';

const BASE_URL = 'https://localhost:7174/api/';

@Injectable({
  providedIn: 'root'
})
export class Consume_energyService {

  consumeEnergyRequestSubject$ = new BehaviorSubject<ConsumeEnergyRequest | null>(null);

  constructor(private http: HttpClient) {}

  setConsumeEnergyRequest(req: ConsumeEnergyRequest) {
    this.consumeEnergyRequestSubject$.next(req);
  }

  consumeEnergySearch(req: ConsumeEnergyRequest){
      return this.http.post<ConsumeEnergyResponse>(
        `${BASE_URL}gov/electricity/consumption/search`,
        req
      );
  }
  consumeEnergyUpdate(){
    return this.http.put<any>(
      `${BASE_URL}gov/electricity/consumption/update`,null
    );
}
}
