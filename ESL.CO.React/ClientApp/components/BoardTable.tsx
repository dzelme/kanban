import * as React from 'react';
import ColumnFill from './ColumnFill';
import { Board, CardColor } from './Interfaces';

const styleColumns = {
    width: ''
}

export default class BoardTable extends React.Component<{ board: Board }> {

    public render() {

        let columnCount = this.props.board.columns.length;

        return <div>{
            this.props.board.columns.map((column, index) =>
                <section style={BoardTable.columnSize(this.props.board.columns.length)} key={index}><ColumnFill column={column} colorList={this.props.board.cardColors} /></section>
            )

        }</div>
    }

    private static columnSize(columnCount: number) {

        styleColumns.width = (100 / columnCount) + '%'

        return styleColumns;
    }
}
