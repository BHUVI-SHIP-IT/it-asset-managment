import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Depreciation } from './depreciation';

describe('Depreciation', () => {
  let component: Depreciation;
  let fixture: ComponentFixture<Depreciation>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Depreciation],
    }).compileComponents();

    fixture = TestBed.createComponent(Depreciation);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
