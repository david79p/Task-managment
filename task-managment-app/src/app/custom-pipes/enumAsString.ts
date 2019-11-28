import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'eNumAsString'
})

export class ENumAsStringPipe implements PipeTransform {
    transform(value: number, enumType: any): any {
        return enumType[value];
    }
}