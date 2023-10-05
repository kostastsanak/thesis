import { Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { BehaviorSubject, Subscription } from 'rxjs';
import { MaterialFormModule } from 'src/app/shared/material-form.module';
import { MaterialMinModule } from 'src/app/shared/material-min.module';
import { ConsumeEnergyRequest, ConsumeEnergyResponse, Consumption } from '../model/consumeEnergy.model';
import { Consume_energyService } from '../service/consume_energy.service';
import { Pagging } from 'src/app/core/models/core.models';

@Component({
  selector: 'app-consumeEnergyTable',
  templateUrl: './consumeEnergyTable.component.html',
  styleUrls: ['./consumeEnergyTable.component.scss'],
  imports: [MaterialMinModule, MaterialFormModule],
  standalone: true
})
export class ConsumeEnergyTableComponent implements OnInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  @Output() correctRequest: EventEmitter<boolean> = new EventEmitter(true);
  myPaginator!: MatPaginator;
  consumeEnergyRequest$ = new BehaviorSubject<ConsumeEnergyRequest | null>(null);
  consumeEnergyRequest!: ConsumeEnergyRequest;
  pagging$ = new BehaviorSubject<Pagging | null>(null);
  pagging!: Pagging;

  consumption$ = new BehaviorSubject<Consumption[] | null>(null);
  consumption!: Consumption[];
  subscriptions: Subscription = new Subscription();
  dataSource!: MatTableDataSource<any>;
  columnsToDisplay = ["area", "energy_mwh", "date","update"];

  constructor(private consumeEnergyService: Consume_energyService) { }

  ngOnInit() {
    this.setRequest();
    this.requestGlobalSearch();
    this.subscriptions.add(
      this.pagging$.subscribe((pagging)=> {
      this.pagging = (pagging as Pagging);
      })
    );
  }


  setRequest() {
    this.subscriptions.add(
      this.consumeEnergyService.consumeEnergyRequestSubject$.subscribe((data) => {
        if (data) {
          this.consumeEnergyRequest = data;
          this.consumeEnergyRequest$.next(this.consumeEnergyRequest);
        }
      })
    );
  }
  requestGlobalSearch() {
    this.subscriptions.add(
      this.consumeEnergyRequest$.subscribe(() => {
        this.subscriptions.add(
          this.consumeEnergyService
            .consumeEnergySearch(this.consumeEnergyRequest)
            .subscribe((data) => {
              if (data && data.payload.consumptions) {
                this.correctRequest.emit(true)
                this.consumption$.next(data.payload.consumptions);
                this.pagging$.next(data.payload.pagging);
              }else{
                this.correctRequest.emit(false)
              }
            })
        );
      })
    );
  }


  ngAfterViewInit(): void {
    setTimeout(() => {
      this.myPaginator = this.paginator;
      this.pagging != null ? this.pagging.currentPage -1 : 0;
      this.subscriptions.add(
        this.consumption$.subscribe((data) => {
          if (data != null) {
            this.dataSource = new MatTableDataSource(data);
            this.dataSource.sort = this.sort;
          }
        })
      );
      this.subscriptions.add(
        this.myPaginator.page.subscribe((data) => {
          var pageNumber = data.pageIndex + 1;
          setTimeout(() => {
            //if change page size then go back to first page
            if (
              this.pagging &&
              this.pagging.entriesPerPage != this.myPaginator.pageSize
              ) {
                this.consumeEnergyRequest.page = 1;
              this.paginator.pageIndex = 0;
            } else {
              this.consumeEnergyRequest.page = pageNumber;
            }
            this.consumeEnergyRequest.limit = this.myPaginator.pageSize;
            this.consumeEnergyRequest$.next(this.consumeEnergyRequest);
          });
        })
      );
    });
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }
  updateForm(){
    this.consumeEnergyService.consumeEnergyUpdate().subscribe();
  }
  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }
}
