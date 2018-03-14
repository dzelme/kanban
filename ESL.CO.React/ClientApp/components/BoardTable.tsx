import * as React from 'react';
import ColumnTitle from './ColumnTitle';
import ColumnFill from './ColumnFill';
import Ticket from './Ticket';
import { Board, Issue } from './Interfaces';


export default class BoardTable extends React.Component<{ board: Board, boardTime: number }> {

    public render() {

        let columnCount = this.props.board.columns.length;

        return <div>{

            this.props.board.columns.map((column, index) =>
                <section className='column' key={index}><ColumnFill column={column} /></section>
                )

        }</div>
    }
}
