import { Component, OnInit } from '@angular/core';
import { Consume_energyService } from '../service/consume_energy.service';
import { UntypedFormGroup, UntypedFormBuilder, UntypedFormControl, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { MaterialFormModule } from 'src/app/shared/material-form.module';
import { DAYS_ARRAY, MONTHS_ARRAY} from 'src/app/core/utilities';

@Component({
  selector: 'app-consumeEnergyForm',
  templateUrl: './consumeEnergyForm.component.html',
  styleUrls: ['./consumeEnergyForm.component.scss'],
  imports: [MaterialFormModule],
  standalone : true
})
export class ConsumeEnergyFormComponent implements OnInit {
  subscriptions: Subscription = new Subscription();
  form: UntypedFormGroup | undefined;
  inputAppearance = "outline";
  days = DAYS_ARRAY
  months = MONTHS_ARRAY
  inputColor = "";
  constructor(private fb: UntypedFormBuilder, private consumeEnergyService: Consume_energyService) { 
    this.initForm();
  }

  ngOnInit() {
    this.onSubmit();
  }

  initForm() {
    this.form = this.fb.group({
      cityID: new UntypedFormControl(null),
      city: new UntypedFormControl(null),
      year: new UntypedFormControl(null),
      month: new UntypedFormControl(null),
      day: new UntypedFormControl(null),
      groupByYear: new UntypedFormControl(null),
      groupByMonth: new UntypedFormControl(null),
      groupByCity: new UntypedFormControl(null),
      limit: new UntypedFormControl(10),
      page: new UntypedFormControl(1),
      noPagging: new UntypedFormControl(null)
    });
  }
  resetForm() {
    this.form!.reset();
    this.form!.get("limit")!.setValue(10);
    this.form!.get("page")!.setValue(0);
    this.form!.get("year")!.setValue(null);
    this.form!.get("month")!.setValue(null);

  }
  onSubmit() {
    if(this.form!.value.city === "")  {this.form!.value.city = null;}
    this.consumeEnergyService.setConsumeEnergyRequest(this.form!.value);
  }
  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

}
