/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { TotalEnergyProductionService } from './totalEnergyProduction.service';

describe('Service: TotalEnergyProduction', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [TotalEnergyProductionService]
    });
  });

  it('should ...', inject([TotalEnergyProductionService], (service: TotalEnergyProductionService) => {
    expect(service).toBeTruthy();
  }));
});
