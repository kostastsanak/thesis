import { Component, OnInit } from '@angular/core';
import { MaterialFormModule } from 'src/app/shared/material-form.module';
import { UntypedFormGroup, UntypedFormBuilder, UntypedFormControl, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { TotalEnergyProductionService } from '../service/totalEnergyProduction.service';
import {  Router } from '@angular/router';


@Component({
  selector: 'app-totalEnergyProductionForm',
  templateUrl: './totalEnergyProductionForm.component.html',
  styleUrls: ['./totalEnergyProductionForm.component.scss'],  
  imports: [MaterialFormModule],
  standalone:true
})
export class TotalEnergyProductionFormComponent implements OnInit {
  subscriptions: Subscription = new Subscription();
  form: UntypedFormGroup | undefined;
  inputAppearance = "outline";
  inputColor = "primary";
  showExtraFields:boolean = false;
  fuelTypes=[
    {
      fuelID:1,
      fuelValue:"ΛΙΓΝΙΤΗΣ"
    },
    {
      fuelID:2,
      fuelValue:"ΑΠΕ"
    },    {
      fuelID:3,
      fuelValue:"ΥΔΡΟΗΛΕΚΤΡΙΚΑ"//
    },
    {
      fuelID:4,
      fuelValue:"ΑΕΡΙΟ"
    },
    {
      fuelID:5,
      fuelValue:"ΦΥΣΙΚΟ ΑΕΡΙΟ"
    },
    {
      fuelID:7,
      fuelValue:"ΑΙΟΛΙΚΑ"//
    },
    {
      fuelID:8,
      fuelValue:"ΚΑΘΑΡΕΣ ΕΙΣΑΓΩΓΕΣ (ΕΙΣΑΓΩΓΕΣ-ΕΞΑΓΩΓΕΣ)"
    },
    {
      fuelID:9,
      fuelValue:"ΣΥΝΟΛΟ"
    }
  ]
  constructor(private router:Router ,private fb: UntypedFormBuilder, private totalEnergyProductionService: TotalEnergyProductionService) {
    if (this.router.url.includes('renewableEnergy')) {
      this.showExtraFields = true;
    }
    this.initForm();
  }

  ngOnInit() {
    this.onSubmit();
  }

  initForm() {
    this.form = this.fb.group({
      fuelIDs: new UntypedFormControl(this.showExtraFields ? [3,7] : [1,2,3,4,5,7,8,9]),
      fuel: new UntypedFormControl(null),
      year: new UntypedFormControl(null),
      month: new UntypedFormControl(null),
      day: new UntypedFormControl(null),
      groupByFuel: new UntypedFormControl(null),
      groupByYear: new UntypedFormControl(null),
      groupByMonth: new UntypedFormControl(null),
      limit: new UntypedFormControl(10),
      page: new UntypedFormControl(1),
      noPagging: new UntypedFormControl(null)
    });
  }
  resetForm() {
    this.form!.reset();
    this.form!.get("fuelIDs")!.setValue(this.showExtraFields ? [3,7] : [1,2,3,4,5,7,8,9]);
    this.form!.get("limit")!.setValue(10);
    this.form!.get("page")!.setValue(0);
    // this.form!.get("year")!.setValue(null);
    // this.form!.get("month")!.setValue(null);

  }
  onSubmit() {
    if(this.form!.value.city === "")  {this.form!.value.city = null;}
    this.totalEnergyProductionService.setTotalEnergyProductionRequest(this.form!.value);
  }
  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }
  
}
