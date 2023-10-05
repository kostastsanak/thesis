/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { Consume_energyService } from './consume_energy.service';

describe('Service: Consume_energy', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [Consume_energyService]
    });
  });

  it('should ...', inject([Consume_energyService], (service: Consume_energyService) => {
    expect(service).toBeTruthy();
  }));
});
