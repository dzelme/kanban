﻿import * as React from 'react';
import BoardName from './BoardName';
import BoardTable from './BoardTable';
import { Value, Board } from './Interfaces';

interface ColumnReaderState {
    boardlist: Value[];
    currentIndex: number;
    boardId: number;
    board: Board;
    boardChanged: boolean;
    loading: boolean;
}

// test when no appSettings.json - currently creates error @boardId: this.props.boardlist[0].id
// error because generated file hass all boards with visibility false
export default class ColumnReader extends React.Component<{ boardlist: Value[] }, ColumnReaderState> {
    refreshTimer: number;

    constructor(props) {
        super(props);
        this.state = {
            boardlist: this.props.boardlist,
            currentIndex: 0,
            boardId: this.props.boardlist[0].id,
            board: {
                id: 0, name: "", fromCache: false, message: "", columns: [], rows: [], hasChanged: false
            },
            boardChanged: false,
            loading: true
        };

        this.nextSlide = this.nextSlide.bind(this);


        fetch('api/SampleData/BoardData?ID=' + this.state.boardId)
            .then(response => response.json() as Promise<Board>)
            .then(data => {
                this.setState({ board: data, loading: false, boardChanged: true }, this.RefreshRate);
            });
    }

    nextSlide() {

        var index;

        if (this.state.currentIndex == (this.state.boardlist.length - 1)) {
            index = 0;
        }
        else {
            index = this.state.currentIndex + 1;
        }


        const newState = {
            currentIndex: index,
            boardId: this.state.boardlist[index].id,
            boardChanged: false
        }

        this.setState(newState, this.boardLoad);
    }


    slideShow() {

        setTimeout(this.nextSlide, this.state.boardlist[this.state.currentIndex].timeShown);

    }

    boardLoad() {

        clearInterval(this.refreshTimer);

        fetch('api/SampleData/BoardData?ID=' + this.state.boardId)
            .then(response => response.json() as Promise<Board>)
            .then(data => {
                if (data.id == this.state.boardId) {                                            //nonem problemu, ja fetch beidzas pec tam, kad jau jauns boards izvelets

                    if (this.state.board.id == data.id && data.hasChanged == false) {           //ja tiek nolasits tas pats boards un nav mainijies

                        this.setState({ board: data, boardChanged: false }, this.RefreshRate);

                    }
                    else {

                        this.setState({ board: data, boardChanged: true }, this.RefreshRate);
                    }

                }

            });

    }

    RefreshRate() {

        this.refreshTimer = setInterval(
            () => this.boardLoad(),
            this.state.boardlist[this.state.currentIndex].refreshRate
        );

    }

    shouldComponentUpdate(nextProps, nextState) {

        return nextState.boardChanged;
    }

    componentWillUnmount() {
        clearInterval(this.refreshTimer);
    }

    public render() {

        if (this.state.loading) {
            return <h1>Loading...</h1>
        }
        else {

            if (this.state.board.columns.length == 0) {
                return <h1>Error loading!</h1>
            }
            else {

                return <div>

                    <div style={styleCenter}>  <BoardName name={this.state.board.name} fromCache={this.state.board.fromCache} message={this.state.board.message} /></div>
                    <div style={styleCenter}><BoardTable board={this.state.board} boardTime={this.state.boardlist[this.state.currentIndex].timeShown} /></div>

                    {
                        this.slideShow()
                    }

                </div>;
            }
        }

    }
}

const styleCenter = {
    height: '100 %',
    width: '100 %',
    display: 'flex',
    justifyContent: 'center' as 'center',
    alignItems: 'center' as 'center'
}
