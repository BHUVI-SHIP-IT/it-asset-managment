import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AccessoryList } from './accessory-list';

describe('AccessoryList', () => {
  let component: AccessoryList;
  let fixture: ComponentFixture<AccessoryList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AccessoryList],
    }).compileComponents();

    fixture = TestBed.createComponent(AccessoryList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
