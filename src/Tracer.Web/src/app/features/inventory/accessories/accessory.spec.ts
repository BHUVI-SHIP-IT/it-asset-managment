import { TestBed } from '@angular/core/testing';

import { Accessory } from './accessory';

describe('Accessory', () => {
  let service: Accessory;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Accessory);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
